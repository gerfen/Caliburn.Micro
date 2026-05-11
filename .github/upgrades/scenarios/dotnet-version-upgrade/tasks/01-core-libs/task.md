# 01-core-libs: Confirm core libraries require no changes

`Caliburn.Micro.Core` and `Caliburn.Micro.Platform.Core` both target `netstandard2.0`. The assessment confirmed zero issues — no TFM changes, no package issues, no API issues. This task verifies they build cleanly against .NET 10 as-is and serves as the baseline for all downstream groups.

**Done when**: Both projects build without errors and are confirmed compatible as a foundation for the rest of the upgrade.
