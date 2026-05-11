# 05-winui3: Resolve WinUI3 package incompatibility

Group: `Caliburn.Micro.WinUI3`

This project already multi-targets `net9.0-windows10.0.19041.0;net10.0-windows10.0.19041.0` — the net10.0 target is already present. The main issue is the incompatible `Microsoft.Xaml.Behaviors.WinUI.Managed` package and 30 source-incompatible API issues. The TFM may need a minor adjustment (drop net9.0 target or align with the net10.0-only target).

**Done when**: Package incompatibility resolved, API issues fixed, project builds successfully targeting net10.0-windows10.0.19041.0.
