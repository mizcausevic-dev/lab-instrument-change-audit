namespace LabInstrumentChangeAudit.Api;

public static class SampleData
{
    public static readonly LabInstrumentAuditExport Payload = new(
        Snapshots:
        [
            new(
                "inst-core",
                "Core instrument change snapshot",
                "Calibration and firmware lane",
                "BOS-QC-1",
                "WATCH",
                "CURRENT",
                "Instrument Quality",
                8,
                3,
                DateTimeOffset.Parse("2026-05-29T16:10:00Z")
            ),
            new(
                "inst-release",
                "Release control snapshot",
                "Release and attestation lane",
                "RTP-QA-2",
                "CRITICAL",
                "STALE",
                "Quality Systems",
                5,
                2,
                DateTimeOffset.Parse("2026-05-27T10:00:00Z")
            )
        ],
        Gaps:
        [
            new(
                "gap-firmware-review",
                "inst-core",
                "Firmware",
                "high",
                "LC-MS firmware change packet",
                "Firmware changes keep signed impact review and approved rollback notes before instrument reuse.",
                "The current firmware packet is missing rollback evidence and the final approver note.",
                18,
                true
            ),
            new(
                "gap-calibration-window",
                "inst-release",
                "Calibration",
                "high",
                "HPLC calibration window",
                "Release-critical instruments keep fresh calibration evidence inside the approved review window.",
                "A release lane still depends on calibration evidence that is outside the acceptable freshness window.",
                33,
                true
            ),
            new(
                "gap-sop-ack",
                "inst-release",
                "Training",
                "high",
                "Operator SOP acknowledgment",
                "Operators acknowledge the current SOP revision before running post-change samples.",
                "One post-change operator run is missing the latest SOP acknowledgment evidence.",
                26,
                true
            ),
            new(
                "gap-maintenance-link",
                "inst-core",
                "Maintenance",
                "medium",
                "Preventive maintenance bridge",
                "Preventive maintenance packets stay linked to the approved change record.",
                "The maintenance bridge is missing for one changed instrument and weakens the audit trail.",
                14,
                false
            ),
            new(
                "gap-audit-trace",
                "inst-core",
                "Audit",
                "medium",
                "Audit-trace export continuity",
                "Change events keep a continuous timestamped audit export before review closure.",
                "One audit-trace export omitted an intermediate signoff event and needs regeneration.",
                11,
                false
            ),
            new(
                "gap-attestation-packet",
                "inst-release",
                "Release",
                "high",
                "Instrument change attestation packet",
                "Instrument changes retain complete final attestation packets before QA closes the lane.",
                "The attestation packet is missing a reviewer signature and linked evidence proof.",
                16,
                true
            )
        ]
    );

    public static readonly IReadOnlyList<InstrumentLanePacket> InstrumentLane =
    [
        new(
            "firmware-controls",
            "Firmware and configuration lane",
            "Instrument Quality",
            "red",
            "Firmware review, rollback evidence, and release-control continuity",
            "Close the rollback evidence gap before another configuration push clears review.",
            "Firmware posture is not strong enough for blind reuse of the affected instrument."
        ),
        new(
            "calibration-controls",
            "Calibration and readiness lane",
            "Metrology Operations",
            "red",
            "Calibration freshness, drift review, and downstream release dependencies",
            "Refresh the stale calibration packet and rerun the readiness dependency check.",
            "One stale calibration window is blocking a credible release posture."
        ),
        new(
            "training-controls",
            "Training and SOP lane",
            "Lab Operations",
            "yellow",
            "Operator acknowledgment, SOP revision proof, and controlled reuse timing",
            "Close the missing SOP acknowledgment before the next post-change operator run.",
            "Training posture is recoverable if the acknowledgment packet lands in the next window."
        ),
        new(
            "release-controls",
            "Final audit lane",
            "Quality Systems",
            "red",
            "QA signoff, attestation completeness, and change-packet timing",
            "Close the missing reviewer signature and evidence-link gaps before final closure.",
            "The final audit packet is still incomplete."
        )
    ];

    public static readonly IReadOnlyList<AuditPacket> AuditPackets =
    [
        new(
            "LICA-12",
            "Firmware change packet",
            "Instrument Quality",
            "red",
            59,
            "Rollback evidence and final approver note are still missing.",
            "Do not clear this instrument for reuse until rollback and approver proof are attached.",
            7
        ),
        new(
            "LICA-18",
            "Calibration review packet",
            "Metrology Operations",
            "red",
            63,
            "The release lane still depends on stale calibration evidence.",
            "Block downstream reuse and reroute this unit through calibration review.",
            10
        ),
        new(
            "LICA-21",
            "Training acknowledgment packet",
            "Lab Operations",
            "yellow",
            78,
            "The latest SOP acknowledgment is still missing on one operator packet.",
            "Closure can recover if the operator acknowledgment lands in the next cycle.",
            13
        ),
        new(
            "LICA-27",
            "Final attestation packet",
            "Quality Systems",
            "yellow",
            74,
            "The attestation packet still needs a reviewer signature and linked proof.",
            "Audit posture is recoverable if the final signature lands before the next checkpoint.",
            18
        )
    ];
}
