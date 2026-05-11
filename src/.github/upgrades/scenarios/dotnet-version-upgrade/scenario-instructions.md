# .NET Version Upgrade - Scenario Instructions

## Target Configuration
- **Target Framework**: .NET 10.0 (LTS)
- **Solution**: Caliburn.Micro.sln
- **Scope**: Full solution upgrade

## Source Control
- **Source Branch**: master
- **Working Branch**: upgrade-to-NET10
- **Repository Root**: D:\temp\Caliburn.Micro Projects\dotNet10\Caliburn.Micro

## Preferences

### Flow Mode
**Automatic** — Run end-to-end, surface assessment and plan as I go, only pause when blocked or needing user input.

## User Preferences

### Technical Preferences
*(User preferences will be recorded here as they are expressed)*

### Execution Style
*(Execution preferences will be recorded here as they are expressed)*

### Custom Instructions
*(Task-specific custom instructions will be recorded here)*

## Strategy
**Selected**: Hybrid — 6 platform groups in dependency order
**Rationale**: Heterogeneous solution with mixed platforms (WPF, MAUI, Avalonia, WinUI3), varying framework versions, and very different risk profiles per group.

### Execution Constraints
- Groups execute in dependency order: core-libs → avalonia/core.tests → maui → platform-wpf → winui3 → xamarin-forms → validation
- Cross-group integration: each group must build cleanly before the next dependent group starts
- Retain net462 target in Platform project — do NOT remove it
- xunit deprecated package: resolve in each test project as they are upgraded
- Full solution validation is the final task — must pass before completion

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Phase
- **Pace**: Standard

## Key Decisions Log
- Hybrid strategy selected — heterogeneous platforms with distinct risk profiles per group
- net462 target in Caliburn.Micro.Platform must be preserved (user has .NET Framework targets)
- Fully automatic mode — only stop if genuinely blocked
