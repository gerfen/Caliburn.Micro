# .NET 10 Upgrade Plan

## Overview

**Target**: .NET 10.0 (LTS) for all applicable projects
**Scope**: 11 projects, 9 require TFM changes, ~43k LOC, 286 files, ~1,633 LOC impacted

### Selected Strategy
**Hybrid** — Solution segmented into 6 groups with per-group strategies.
**Rationale**: Heterogeneous solution with mixed platforms (WPF, MAUI, Avalonia, WinUI3), varying current framework versions (net8.0, net9.0, netstandard2.0, net462), and very different complexity profiles. Core libraries require no changes; platform-specific projects have distinct risk profiles and must be upgraded independently.

## Tasks

---

### 01-core-libs: Confirm core libraries require no changes

`Caliburn.Micro.Core` and `Caliburn.Micro.Platform.Core` both target `netstandard2.0`. The assessment confirmed zero issues — no TFM changes, no package issues, no API issues. This task verifies they build cleanly against .NET 10 as-is and serves as the baseline for all downstream groups.

**Done when**: Both projects build without errors and are confirmed compatible as a foundation for the rest of the upgrade.

---

### 02-avalonia: Upgrade Avalonia projects to net10.0

Group: `Caliburn.Micro.Avalonia`, `Caliburn.Micro.Avalonia.Tests`, `Caliburn.Micro.Core.Tests`

These three projects currently target `net8.0`. The assessment shows low difficulty — simple TFM bumps, 1 deprecated package (`xunit`), and a small number of API source-incompatible issues (2 in Avalonia.Tests, 4 in Core.Tests). All Avalonia packages (11.3.5) are compatible.

**Done when**: All three projects target `net10.0`, the deprecated xunit package is resolved, API issues are fixed, and all tests pass.

---

### 03-maui: Upgrade MAUI project to net10.0

Group: `Caliburn.Micro.Maui`, `Caliburn.Micro.Maui.Tests`

These projects currently target `net9.0` and its platform variants. The assessment flags 515 source-incompatible API issues in Maui (primarily `BindableProperty` and `BindableObject` API changes) and 38 in Maui.Tests. The `Microsoft.Maui.Controls` package (9.0.120) is compatible. Maui.Tests has one deprecated package (`xunit`). This is the highest code-change-volume group.

**Done when**: Maui project targets `net10.0` (and platform variants), Maui.Tests targets `net10.0`, all BindableProperty/BindableObject API issues resolved, deprecated package resolved, and all tests pass.

---

### 04-platform-wpf: Upgrade Platform WPF project to net10.0

Group: `Caliburn.Micro.Platform`, `Caliburn.Micro.Platform.Tests`

`Platform` is the most complex project: it multi-targets `net462;uap10.0.19041;net9.0-android;net9.0-ios;net8.0-windows;net9.0-windows`. The assessment identifies 969 API issues (primarily WPF `DependencyProperty`/`DependencyObject` binary incompatibilities) and 1 incompatible package (`Microsoft.Xaml.Behaviors.Wpf` — suggested downgrade to 1.1.39). `Platform.Tests` (net8.0-windows) has 73 API issues, 2 incompatible packages, and 1 deprecated package.

The .NET Framework 4.6.2 target (`net462`) must be retained as-is. Only the .NET versioned targets should be upgraded.

**Done when**: Platform targets updated (net8.0-windows → net10.0-windows, net9.0-windows → net10.0-windows, net9.0-android/ios → net10.0-android/ios), Platform.Tests targets net10.0-windows, Xaml.Behaviors.Wpf package resolved, WPF API issues addressed, and solution builds.

---

### 05-winui3: Resolve WinUI3 package incompatibility

Group: `Caliburn.Micro.WinUI3`

This project already multi-targets `net9.0-windows10.0.19041.0;net10.0-windows10.0.19041.0` — the net10.0 target is already present. The main issue is the incompatible `Microsoft.Xaml.Behaviors.WinUI.Managed` package and 30 source-incompatible API issues. The TFM may need a minor adjustment (drop net9.0 target or align with the net10.0-only target).

**Done when**: Package incompatibility resolved, API issues fixed, project builds successfully targeting net10.0-windows10.0.19041.0.

---

### 06-xamarin-forms: Update Xamarin.Forms project

Group: `Caliburn.Micro.Xamarin.Forms`

This project targets `netstandard2.0` and has 2 behavioral-change API issues. The `Xamarin.Forms` 5.0.0.2662 package is flagged as "functionality included with framework reference" — it should be removed in favor of the appropriate MAUI package or removed if the project is being deprecated. This is a low-risk group.

**Done when**: Xamarin.Forms package situation resolved (removed or replaced), behavioral-change APIs addressed, project builds cleanly.

---

### 07-solution-validation: Full solution build and test validation

After all platform groups are upgraded, perform a full solution build and run all test suites to confirm there are no cross-group regressions, integration issues, or remaining incompatibilities.

**Done when**: Full solution builds without errors or warnings related to the upgrade; all test projects pass; no remaining package incompatibilities or deprecated package references.
