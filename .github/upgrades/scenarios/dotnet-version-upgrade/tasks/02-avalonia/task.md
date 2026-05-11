# 02-avalonia: Upgrade Avalonia projects to net10.0

Group: `Caliburn.Micro.Avalonia`, `Caliburn.Micro.Avalonia.Tests`, `Caliburn.Micro.Core.Tests`

These three projects currently target `net8.0`. The assessment shows low difficulty — simple TFM bumps, 1 deprecated package (`xunit`), and a small number of API source-incompatible issues (2 in Avalonia.Tests, 4 in Core.Tests). All Avalonia packages (11.3.5) are compatible.

**Done when**: All three projects target `net10.0`, the deprecated xunit package is resolved, API issues are fixed, and all tests pass.
