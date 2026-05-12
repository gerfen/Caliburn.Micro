namespace Caliburn.Micro {
    using System;
using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A custom IoC container which integrates with WinUI 3 (Windows App SDK) and properly registers all Caliburn.Micro services.
    /// </summary>
    public class WinUI3Container : SimpleContainer {
        /// <summary>
        /// Registers the Caliburn.Micro WinUI 3 services with the container.
        /// </summary>
        public void RegisterWinUIServices() {
            RegisterInstance(typeof (SimpleContainer), null, this);
            RegisterInstance(typeof (WinUI3Container), null, this);

            if (!HasHandler(typeof (IEventAggregator), null)) {
                RegisterSingleton(typeof (IEventAggregator), null, typeof (EventAggregator));
            }

            if (!HasHandler(typeof (IWindowManager), null)) {
                RegisterSingleton(typeof (IWindowManager), null, typeof (WindowManager));
            }
        }

        /// <summary>
        /// Registers the Caliburn.Micro navigation service with the container.
        /// </summary>
        /// <param name="rootFrame">The application root frame.</param>
        /// <param name="treatViewAsLoaded">if set to <c>true</c> [treat view as loaded].</param>
        /// <param name="cacheViewModels">if set to <c>true</c> then navigation service cache view models for resuse.</param>
        public INavigationService RegisterNavigationService(Frame rootFrame, bool treatViewAsLoaded = false, bool cacheViewModels = false) {
            if (HasHandler(typeof (INavigationService), null))
                return this.GetInstance<INavigationService>(null);

            if (rootFrame == null)
                throw new ArgumentNullException("rootFrame");
var frameAdapter = cacheViewModels ? (INavigationService)
    new CachingFrameAdapter(rootFrame, treatViewAsLoaded) :
    new FrameAdapter(rootFrame, treatViewAsLoaded);

            RegisterInstance(typeof (INavigationService), null, frameAdapter);

            return frameAdapter;
        }

        /// <summary>
        /// Registers the Caliburn.Micro sharing service with the container.
        /// </summary>
        public ISharingService RegisterSharingService() {
            if (HasHandler(typeof (ISharingService), null))
                return this.GetInstance<ISharingService>(null);

            var sharingService = new SharingService();

            RegisterInstance(typeof (ISharingService), null, sharingService);

            return sharingService;
        }

    }
}
