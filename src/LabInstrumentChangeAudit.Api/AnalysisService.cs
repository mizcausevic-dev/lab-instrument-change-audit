namespace LabInstrumentChangeAudit.Api;

public static class AnalysisService
{
    public static InstrumentAuditPostureReport Analyze(LabInstrumentAuditExport payload)
    {
        var findings = new List<LabInstrumentAuditFinding>();

        foreach (var gap in payload.Gaps)
        {
            findings.Add(new LabInstrumentAuditFinding(
                GapCode(gap.ControlFamily),
                gap.Severity,
                gap.Subject,
                gap.ObservedState,
                OwnerForGap(gap.ControlFamily)));
        }

        var snapshots = payload.Snapshots.Count;
        var currentSnapshots = payload.Snapshots.Count(snapshot => snapshot.SnapshotStatus == "CURRENT");
        var gaps = payload.Gaps.Count;
        var blockingGaps = payload.Gaps.Count(gap => gap.BlocksRelease);
        var controlRisks = payload.Gaps.Count(gap => gap.ControlFamily is "Firmware" or "Calibration" or "Training" or "Maintenance");
        var auditRisks = payload.Gaps.Count(gap => gap.ControlFamily is "Audit" or "Release");
        var ok = blockingGaps == 0;

        return new InstrumentAuditPostureReport(
            snapshots,
            currentSnapshots,
            gaps,
            blockingGaps,
            controlRisks,
            auditRisks,
            findings,
            ok
        );
    }

    public static object Summary()
    {
        var report = Analyze(SampleData.Payload);
        return new
        {
            snapshots = report.Snapshots,
            currentSnapshots = report.CurrentSnapshots,
            gaps = report.Gaps,
            blockingGaps = report.BlockingGaps,
            controlRisks = report.ControlRisks,
            auditRisks = report.AuditRisks,
            ok = report.Ok
        };
    }

    private static string GapCode(string family) => family switch
    {
        "Firmware" => "firmware-change-gap",
        "Calibration" => "instrument-calibration-gap",
        "Training" => "sop-acknowledgment-gap",
        "Maintenance" => "maintenance-bridge-gap",
        "Audit" => "audit-trace-gap",
        "Release" => "change-attestation-gap",
        _ => "instrument-control-gap"
    };

    private static string OwnerForGap(string family) => family switch
    {
        "Firmware" => "Instrument Quality",
        "Calibration" => "Metrology Operations",
        "Training" => "Lab Operations",
        "Maintenance" => "Metrology Operations",
        "Audit" => "Quality Systems",
        "Release" => "Quality Systems",
        _ => "Instrument Quality"
    };
}
