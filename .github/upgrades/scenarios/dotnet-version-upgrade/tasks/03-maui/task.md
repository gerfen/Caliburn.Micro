# 03-maui: Upgrade MAUI project to net10.0

Group: `Caliburn.Micro.Maui`, `Caliburn.Micro.Maui.Tests`

These projects currently target `net9.0` and its platform variants. The assessment flags 515 source-incompatible API issues in Maui (primarily `BindableProperty` and `BindableObject` API changes) and 38 in Maui.Tests. The `Microsoft.Maui.Controls` package (9.0.120) is compatible. Maui.Tests has one deprecated package (`xunit`). This is the highest code-change-volume group.

**Done when**: Maui project targets `net10.0` (and platform variants), Maui.Tests targets `net10.0`, all BindableProperty/BindableObject API issues resolved, deprecated package resolved, and all tests pass.
