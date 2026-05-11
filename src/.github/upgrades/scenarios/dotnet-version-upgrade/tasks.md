# .NET 10 Upgrade Progress

## Overview

Upgrading the Caliburn.Micro solution from mixed frameworks (net8.0, net9.0, netstandard2.0, net462) to .NET 10.0 (LTS) across 11 projects using a Hybrid strategy segmented by platform group. Core libraries (netstandard2.0) require no changes and serve as the baseline. Platform groups (Avalonia, MAUI, WPF, WinUI3, Xamarin.Forms) are upgraded independently in dependency order.

**Progress**: 0/7 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

## Tasks

- 🔲 01-core-libs: Confirm core libraries require no changes
- 🔲 02-avalonia: Upgrade Avalonia projects to net10.0
- 🔲 03-maui: Upgrade MAUI project to net10.0
- 🔲 04-platform-wpf: Upgrade Platform WPF project to net10.0
- 🔲 05-winui3: Resolve WinUI3 package incompatibility
- 🔲 06-xamarin-forms: Update Xamarin.Forms project
- 🔲 07-solution-validation: Full solution build and test validation
