
## [2026-05-11 10:59] 01-core-libs

Confirmed Caliburn.Micro.Core and Caliburn.Micro.Platform.Core (both netstandard2.0) build cleanly with zero errors and zero warnings. No changes required. These libraries serve as the stable baseline for all downstream platform group upgrades.


## [2026-05-11 11:04] 02-avalonia

Upgraded Caliburn.Micro.Avalonia, Caliburn.Micro.Avalonia.Tests, and Caliburn.Micro.Core.Tests from net8.0 to net10.0. Updated xunit from 2.9.2 to 2.9.3 in both test projects. Switched Caliburn.Micro.Core.Tests from legacy MSBuild.Sdk.Extras to Microsoft.NET.Sdk (MSBuild.Sdk.Extras does not support net10.0). Updated global.json SDK version from 9.0.x to 10.0.x. All three projects build cleanly. Tests: 106 passed, 0 failed, 5 pre-existing intentional skips. Transitive vulnerability warning for Tmds.DBus.Protocol 0.21.2 (from Avalonia) noted — not directly addressable here.


## [2026-05-11 11:13] 03-maui

Upgraded Caliburn.Micro.Maui and Caliburn.Micro.Maui.Tests from net9.0 to net10.0. Updated Microsoft.Maui.Controls from 9.0.120 to 10.0.0-rc.2.25504.7. Updated xunit from 2.9.2 to 2.9.3 in Maui.Tests. Updated all platform-conditional TFM strings (net9.0-android/ios/maccatalyst → net10.0-*). The 515 API issues from assessment were false positives scanning stale obj/ generated files — no source changes were needed. Builds with 0 errors (14 pre-existing deprecation warnings for ListView/TextCell MAUI APIs). Tests: 39 passed, 0 failed.

