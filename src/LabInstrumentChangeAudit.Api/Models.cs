namespace LabInstrumentChangeAudit.Api;

public sealed record InstrumentSnapshot(
    string Id,
    string Name,
    string InstrumentLane,
    string Site,
    string Status,
    string SnapshotStatus,
    string Owner,
    int OpenChanges,
    int BlockingChanges,
    DateTimeOffset CollectedAt
);

public sealed record ChangeGap(
    string Id,
    string SnapshotId,
    string ControlFamily,
    string Severity,
    string Subject,
    string ExpectedState,
    string ObservedState,
    int HoursOpen,
    bool BlocksRelease
);

public sealed record InstrumentLanePacket(
    string Id,
    string Lane,
    string Owner,
    string Status,
    string Focus,
    string NextAction,
    string Note
);

public sealed record AuditPacket(
    string PacketId,
    string Lane,
    string Owner,
    string Status,
    int CompletenessScore,
    string Blocker,
    string DecisionNote,
    int ReviewWindowHours
);

public sealed record LabInstrumentAuditExport(
    IReadOnlyList<InstrumentSnapshot> Snapshots,
    IReadOnlyList<ChangeGap> Gaps
);

public sealed record LabInstrumentAuditFinding(
    string Code,
    string Severity,
    string Subject,
    string Message,
    string Owner
);

public sealed record InstrumentAuditPostureReport(
    int Snapshots,
    int CurrentSnapshots,
    int Gaps,
    int BlockingGaps,
    int ControlRisks,
    int AuditRisks,
    IReadOnlyList<LabInstrumentAuditFinding> Findings,
    bool Ok
);
