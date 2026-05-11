# 06-xamarin-forms: Update Xamarin.Forms project

Group: `Caliburn.Micro.Xamarin.Forms`

This project targets `netstandard2.0` and has 2 behavioral-change API issues. The `Xamarin.Forms` 5.0.0.2662 package is flagged as "functionality included with framework reference" — it should be removed in favor of the appropriate MAUI package or removed if the project is being deprecated. This is a low-risk group.

**Done when**: Xamarin.Forms package situation resolved (removed or replaced), behavioral-change APIs addressed, project builds cleanly.
