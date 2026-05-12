using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Caliburn.Micro
{
    /// <summary>
    /// A service that manages windows, dialogs and popups for WinUI3 applications.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Shows a modal dialog for the specified model using a <see cref="ContentDialog"/>.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="ContentDialog"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the primary button was pressed,
        /// <see langword="false"/> if the secondary button was pressed,
        /// <see langword="null"/> if the dialog was dismissed.
        /// </returns>
        Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);

        /// <summary>
        /// Shows a non-modal window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="Window"/>.</param>
        Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);

        /// <summary>
        /// Shows a flyout popup for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="Flyout"/>.</param>
        Task ShowPopupAsync(object rootModel, object context = null, IDictionary<string, object> settings = null);
    }

    /// <summary>
    /// A WinUI3 implementation of <see cref="IWindowManager"/> that manages
    /// windows, <see cref="ContentDialog"/>s, and <see cref="Flyout"/>s.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        /// <summary>
        /// Shows a modal <see cref="ContentDialog"/> for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="ContentDialog"/>.</param>
        /// <returns>
        /// <see langword="true"/> for <see cref="ContentDialogResult.Primary"/>,
        /// <see langword="false"/> for <see cref="ContentDialogResult.Secondary"/>,
        /// <see langword="null"/> for <see cref="ContentDialogResult.None"/>.
        /// </returns>
        public virtual async Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var dialog = CreateDialog(rootModel, context, settings);
            var result = await dialog.ShowAsync();

            return result switch
            {
                ContentDialogResult.Primary => true,
                ContentDialogResult.Secondary => false,
                _ => null
            };
        }

        /// <summary>
        /// Shows a non-modal <see cref="Window"/> for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="Window"/>.</param>
        public virtual async Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var window = await CreateWindowAsync(rootModel, context, settings);
            window.Activate();
        }

        /// <summary>
        /// Shows a <see cref="Flyout"/> for the specified model, attached to the current active window's content.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">Optional settings applied to the <see cref="Flyout"/>.</param>
        public virtual async Task ShowPopupAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var flyout = CreateFlyout(rootModel, context, settings);

            // Flyouts must be attached to a FrameworkElement — use the active window's content
            var owner = GetActiveWindowContent()
                ?? throw new InvalidOperationException(
                    "Cannot show a popup without an active window. Call ShowWindowAsync first.");

            if (rootModel is IActivate activator)
                await activator.ActivateAsync();

            flyout.ShowAt(owner);
        }

        /// <summary>
        /// Creates and initialises a <see cref="Window"/> for the given model.
        /// </summary>
        protected virtual async Task<Window> CreateWindowAsync(object rootModel, object context, IDictionary<string, object> settings)
        {
            var view = ViewLocator.LocateForModel(rootModel, null, context);
            ViewModelBinder.Bind(rootModel, view, context);

            var window = EnsureWindow(rootModel, view);

            if (rootModel is IHaveDisplayName named && !string.IsNullOrEmpty(named.DisplayName))
                window.Title = named.DisplayName;

            ApplySettings(window, settings);

            var conductor = new WindowConductor(rootModel, window);
            await conductor.InitialiseAsync();

            return window;
        }

        /// <summary>
        /// Creates and initialises a <see cref="ContentDialog"/> for the given model.
        /// </summary>
        protected virtual ContentDialog CreateDialog(object rootModel, object context, IDictionary<string, object> settings)
        {
            var view = ViewLocator.LocateForModel(rootModel, null, context);
            ViewModelBinder.Bind(rootModel, view, context);

            var dialog = view as ContentDialog ?? new ContentDialog { Content = view };
            dialog.SetValue(View.IsGeneratedProperty, true);

            if (rootModel is IHaveDisplayName named && !string.IsNullOrEmpty(named.DisplayName))
                dialog.Title = named.DisplayName;

            // XamlRoot is required for ContentDialog in WinUI3
            var xamlRoot = GetActiveWindowContent()?.XamlRoot
                ?? throw new InvalidOperationException(
                    "Cannot show a ContentDialog without an active window. Call ShowWindowAsync first.");

            dialog.XamlRoot = xamlRoot;

            ApplySettings(dialog, settings);

            return dialog;
        }

        /// <summary>
        /// Creates a <see cref="Flyout"/> hosting the view for the given model.
        /// </summary>
        protected virtual Flyout CreateFlyout(object rootModel, object context, IDictionary<string, object> settings)
        {
            var view = ViewLocator.LocateForModel(rootModel, null, context);
            ViewModelBinder.Bind(rootModel, view, context);

            var flyout = new Flyout { Content = view as FrameworkElement };
            flyout.SetValue(View.IsGeneratedProperty, true);

            if (rootModel is IDeactivate deactivatable)
                flyout.Closed += async (s, e) => await deactivatable.DeactivateAsync(true);

            ApplySettings(flyout, settings);

            return flyout;
        }

        /// <summary>
        /// Ensures the view is a <see cref="Window"/> or wraps it in one.
        /// </summary>
        protected virtual Window EnsureWindow(object model, object view)
        {
            if (view is Window window)
                return window;

            // Window is not a DependencyObject in WinUI3, so IsGeneratedProperty cannot be set on it.
            return new Window { Content = view as UIElement };
        }

        /// <summary>
        /// Returns the <see cref="FrameworkElement"/> content of the currently active window,
        /// used to anchor <see cref="ContentDialog"/> and <see cref="Flyout"/> instances.
        /// </summary>
        protected virtual FrameworkElement GetActiveWindowContent()
        {
            // WindowManager tracks windows opened via ShowWindowAsync.
            // For single-window apps the main window content is sufficient.
            // Override this method if your app manages multiple windows.
            return null;
        }

        /// <summary>
        /// Applies a dictionary of named property values to a target object via reflection.
        /// </summary>
        protected bool ApplySettings(object target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings == null)
                return false;

            var type = target.GetType();
            foreach (var pair in settings)
            {
                var property = type.GetProperty(pair.Key);
                property?.SetValue(target, pair.Value, null);
            }

            return true;
        }
    }
}
