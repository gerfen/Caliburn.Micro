using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;

namespace Caliburn.Micro
{
    /// <summary>
    /// Manages the lifecycle of a WinUI3 window and its associated view model,
    /// keeping window close and view model deactivation in sync.
    /// </summary>
    public class WindowConductor
    {
        private bool deactivatingFromView;
        private bool deactivatingFromViewModel;
        private bool actuallyClosing;
        private readonly Window view;
        private readonly AppWindow appWindow;
        private readonly object model;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowConductor"/> class.
        /// </summary>
        /// <param name="model">The view model associated with the window.</param>
        /// <param name="view">The WinUI3 window being managed.</param>
        public WindowConductor(object model, Window view)
        {
            this.model = model;
            this.view = view;
            this.appWindow = view.AppWindow;
        }

        /// <summary>
        /// Activates the view model and wires up window/view model lifetime events.
        /// </summary>
        public async Task InitialiseAsync()
        {
            if (model is IActivate activator)
            {
                await activator.ActivateAsync();
            }

            if (model is IDeactivate deactivatable)
            {
                view.Closed += Closed;
                deactivatable.Deactivated += Deactivated;
            }

            if (model is IGuardClose)
            {
                // Window.Closing does not exist in WinUI3 — use AppWindow.Closing instead
                appWindow.Closing += Closing;
            }
        }

        /// <summary>
        /// Handles the window's <see cref="Window.Closed"/> event.
        /// Deactivates the view model unless the close was already triggered by it.
        /// </summary>
        private async void Closed(object sender, WindowEventArgs e)
        {
            view.Closed -= Closed;
            appWindow.Closing -= Closing;

            if (deactivatingFromViewModel)
                return;

            var deactivatable = (IDeactivate)model;
            deactivatingFromView = true;
            await deactivatable.DeactivateAsync(true);
            deactivatingFromView = false;
        }

        /// <summary>
        /// Handles the view model's <see cref="IDeactivate.Deactivated"/> event.
        /// Closes the window unless the deactivation was already triggered by it.
        /// </summary>
        private Task Deactivated(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
                return Task.FromResult(false);

            ((IDeactivate)model).Deactivated -= Deactivated;

            if (deactivatingFromView)
                return Task.FromResult(true);

            deactivatingFromViewModel = true;
            actuallyClosing = true;
            view.Close();
            actuallyClosing = false;
            deactivatingFromViewModel = false;

            return Task.FromResult(true);
        }

        /// <summary>
        /// Handles the <see cref="AppWindow.Closing"/> event.
        /// Allows the view model to cancel the close via <see cref="IGuardClose.CanCloseAsync"/>.
        /// </summary>
        private async void Closing(object sender, AppWindowClosingEventArgs e)
        {
            if (actuallyClosing)
            {
                actuallyClosing = false;
                return;
            }

            // Cancel immediately — we'll re-evaluate asynchronously
            e.Cancel = true;

            var guard = (IGuardClose)model;
            var canClose = await guard.CanCloseAsync(CancellationToken.None);

            if (!canClose)
                return;

            actuallyClosing = true;
            view.Close();
        }
    }
}
