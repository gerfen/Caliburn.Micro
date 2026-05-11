
## [2026-05-11 10:59] 01-core-libs

Confirmed Caliburn.Micro.Core and Caliburn.Micro.Platform.Core (both netstandard2.0) build cleanly with zero errors and zero warnings. No changes required. These libraries serve as the stable baseline for all downstream platform group upgrades.


## [2026-05-11 11:04] 02-avalonia

Upgraded Caliburn.Micro.Avalonia, Caliburn.Micro.Avalonia.Tests, and Caliburn.Micro.Core.Tests from net8.0 to net10.0. Updated xunit from 2.9.2 to 2.9.3 in both test projects. Switched Caliburn.Micro.Core.Tests from legacy MSBuild.Sdk.Extras to Microsoft.NET.Sdk (MSBuild.Sdk.Extras does not support net10.0). Updated global.json SDK version from 9.0.x to 10.0.x. All three projects build cleanly. Tests: 106 passed, 0 failed, 5 pre-existing intentional skips. Transitive vulnerability warning for Tmds.DBus.Protocol 0.21.2 (from Avalonia) noted — not directly addressable here.


## [2026-05-11 11:13] 03-maui

Upgraded Caliburn.Micro.Maui and Caliburn.Micro.Maui.Tests from net9.0 to net10.0. Updated Microsoft.Maui.Controls from 9.0.120 to 10.0.0-rc.2.25504.7. Updated xunit from 2.9.2 to 2.9.3 in Maui.Tests. Updated all platform-conditional TFM strings (net9.0-android/ios/maccatalyst → net10.0-*). The 515 API issues from assessment were false positives scanning stale obj/ generated files — no source changes were needed. Builds with 0 errors (14 pre-existing deprecation warnings for ListView/TextCell MAUI APIs). Tests: 39 passed, 0 failed.


## [2026-05-11 11:17] 04-platform-wpf

Upgraded Caliburn.Micro.Platform (multi-target) and Caliburn.Micro.Platform.Tests to .NET 10. Changes: switched both from MSBuild.Sdk.Extras to Microsoft.NET.Sdk; updated TFMs (removed net8.0-windows + net9.0-windows → net10.0-windows; net9.0-android/ios → net10.0-android/ios; net462 retained); downgraded Microsoft.Xaml.Behaviors.Wpf from 1.1.135 to 1.1.39 (compatible version); added UseWPF=true for WinCore targets; replaced legacy WPF assembly references in tests with UseWPF=true. Build: 0 errors, 0 warnings. Tests: 35 passed, 0 failed. Phase committed to git.


## [2026-05-11 11:25] 05-winui3

Upgraded Caliburn.Micro.WinUI3 to target only net10.0-windows10.0.19041.0 (dropped net9.0 TFM). Updated Microsoft.Xaml.Behaviors.WinUI.Managed from 2.0.9 to 3.0.1. Fixed 3 namespace breaking changes in shared platform source files (ActionMessage.cs, ConventionManager.cs, Parser.cs): EventTriggerBehavior moved from Microsoft.Xaml.Interactions.Core to Microsoft.Xaml.Interactivity in v3.0.1. Build: 0 errors, 1 pre-existing deprecation warning (PackageIconUrl). User confirmed accepting the v3.0.1 breaking changes.


## [2026-05-11 11:25] 06-xamarin-forms

Caliburn.Micro.Xamarin.Forms targets netstandard2.0 — no TFM changes required. Assessed the NuGet.0003 flag for Xamarin.Forms 5.0.0.2662: the "included with framework reference" recommendation does not apply to netstandard2.0 projects; the package is the correct reference for this legacy shim. The System.Uri behavioral change is a .NET 10 runtime edge case, not a compile-time issue. Build: 0 errors, 0 warnings. No file changes needed.

