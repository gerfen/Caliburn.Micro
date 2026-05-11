# 04-platform-wpf: Upgrade Platform WPF project to net10.0

Group: `Caliburn.Micro.Platform`, `Caliburn.Micro.Platform.Tests`

`Platform` is the most complex project: it multi-targets `net462;uap10.0.19041;net9.0-android;net9.0-ios;net8.0-windows;net9.0-windows`. The assessment identifies 969 API issues (primarily WPF `DependencyProperty`/`DependencyObject` binary incompatibilities) and 1 incompatible package (`Microsoft.Xaml.Behaviors.Wpf` — suggested downgrade to 1.1.39). `Platform.Tests` (net8.0-windows) has 73 API issues, 2 incompatible packages, and 1 deprecated package.

The .NET Framework 4.6.2 target (`net462`) must be retained as-is. Only the .NET versioned targets should be upgraded.

**Done when**: Platform targets updated (net8.0-windows → net10.0-windows, net9.0-windows → net10.0-windows, net9.0-android/ios → net10.0-android/ios), Platform.Tests targets net10.0-windows, Xaml.Behaviors.Wpf package resolved, WPF API issues addressed, and solution builds.
