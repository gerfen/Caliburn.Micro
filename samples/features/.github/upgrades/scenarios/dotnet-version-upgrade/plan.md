# .NET 10 Upgrade Plan — Features Solution

## Overview

**Target**: net10.0 (LTS)
**Scope**: 7 projects across 4 platform groups — Avalonia, cross-platform shared, WPF/DotNet variants, and WinUI3/UWP

### Selected Strategy
**Hybrid** — Solution segmented into 5 groups with per-group strategies (all-at-once within each group).
**Rationale**: Projects are all flat (no inter-project dependencies), but vary significantly in complexity, current TFM, platform type, and package risk. Grouping by platform and risk allows focused validation per group.

---

## Tasks

### 01-avalonia: Upgrade Features.Avalonia

Upgrade `Features.Avalonia` from `net8.0` to `net10.0`. This is the lowest-risk project — all Avalonia packages (11.3.5) are already marked compatible with net10.0, and no SDK conversion is required.

Update `TargetFramework` in the project file. Verify build and runtime behaviour.

**Done when**: Project targets net10.0, builds without errors, all Avalonia packages intact.

---

### 02-shared-projects: Upgrade Shared Projects

Upgrade the two shared projects (`Features.CrossPlatform.Shared.shproj`, `Features.Forms.shproj`) currently on `netcore4.5.1`. These are shared project files (`.shproj`) — update the referenced TFM accordingly.

Shared projects don't produce their own output; validate by building any project that consumes them.

**Done when**: Both `.shproj` files reference net10.0 and any consuming projects build successfully.

---

### 03-wpf-dotnetnine: Upgrade Features.NetFive (net9.0-windows → net10.0-windows)

Upgrade `Features.DotNet.csproj` (misnamed "NetFive", currently `net9.0-windows`) to `net10.0-windows`. One package is flagged incompatible — identify and resolve (update or remove). No SDK conversion required.

**Done when**: Project targets `net10.0-windows`, incompatible package resolved, project builds cleanly.

---

### 04-wpf-classic: Upgrade Features.WPF (net48 → net10.0-windows)

This is the most complex project — 23 mandatory issues. It currently targets `net48` and requires:
1. **SDK-style conversion** (`Project.0001`)
2. **TFM upgrade** from `net48` to `net10.0-windows`
3. **Package cleanup**: `Microsoft.Xaml.Behaviors.Wpf` is incompatible (downgrade from 1.1.135 → 1.1.39 per assessment); multiple `System.*` packages are now included in the framework reference and should be removed
4. **`Microsoft.NETCore.Platforms`** is redundant with framework reference — remove

Convert project file first, then update TFM and packages. Build validation required after each major step.

**Done when**: Project uses SDK-style format, targets `net10.0-windows`, all redundant/incompatible packages resolved, project builds cleanly.

---

### 05-winui3-uwp: Upgrade Features.WinUI3 and Features.UWP

These two projects are grouped due to shared WinUI/Windows App SDK concerns.

**Features.WinUI3** (`net8.0-windows10.0.19041.0` → `net10.0-windows10.0.19041.0`):
- `Microsoft.WindowsAppSDK` 1.6 → 2.0.1 (new version available)
- `Microsoft.Xaml.Behaviors.WinUI.Managed` 2.0.9 is incompatible — no suggested replacement, may need removal or manual resolution

**Features.UWP** (`net5.0` → requires special handling):
- UWP targeting net10.0 is not directly supported; this project uses `Microsoft.NETCore.UniversalWindowsPlatform` 6.2.14, which assessment suggests replacing with `Microsoft.WindowsAppSDK=2.0.1` + `Microsoft.Graphics.Win2D=1.1.0` + `Microsoft.Windows.Compatibility=10.0.7`
- SDK-style conversion required (`Project.0001`)
- `Microsoft.Xaml.Behaviors.Uwp.Managed` 2.0.1 is incompatible with no replacement — needs investigation
- **Note**: Full UWP → WinUI3 migration may be out of scope; if unresolvable, document as a known limitation

**Done when**: WinUI3 project builds on net10.0-windows10.0.19041.0 with updated packages. UWP project either successfully migrated or formally documented as a known out-of-scope item.

---

### 06-final-validation: Full Solution Validation

Build the entire Features solution and confirm no cross-project regressions. Run any available tests. Verify all upgraded projects are producing output correctly.

**Done when**: Full solution builds without errors; all previously passing tests continue to pass.

---
