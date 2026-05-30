# Origin

This repo exists to show that Kinetic Gain's control-plane pattern extends cleanly into biotech and diagnostics quality operations.

The primitive is not generic "lab monitoring." The real workflow pressure lives in:

- firmware changes that need rollback and approver proof
- stale calibration evidence that blocks downstream reuse
- SOP acknowledgments that must land before post-change runs
- maintenance and audit-trace continuity that determine whether QA can trust the packet

`lab-instrument-change-audit` packages that pressure into a buyer-readable and operator-readable public surface.
