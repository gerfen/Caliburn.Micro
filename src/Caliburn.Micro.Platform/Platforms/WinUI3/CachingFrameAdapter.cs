using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Caliburn.Micro
{
    /// <summary>
    /// A <see cref="FrameAdapter"/> that caches view model instances across navigation
    /// so the same instance is restored when navigating back or forward.
    /// </summary>
    public class CachingFrameAdapter : FrameAdapter
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(CachingFrameAdapter));

        private readonly Frame frame;
        private readonly List<object> viewModelBackStack = new List<object>();
        private readonly List<object> viewModelForwardStack = new List<object>();

        /// <summary>
        /// Creates an instance of <see cref="CachingFrameAdapter"/>.
        /// </summary>
        public CachingFrameAdapter(Frame frame, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this.frame = frame;
        }

        /// <inheritdoc/>
        protected override async void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            base.OnNavigating(sender, e);

            if (e.Cancel)
                return;

            if (!(frame.Content is FrameworkElement view))
                return;

            var viewModel = view.DataContext;

            switch (e.NavigationMode)
            {
                case NavigationMode.Back:
                    Log.Info("Pushing view model {0} onto the forward stack", viewModel?.GetType().Name ?? "null");
                    viewModelForwardStack.Add(viewModel);
                    break;

                case NavigationMode.Forward:
                case NavigationMode.Refresh:
                    Log.Info("Pushing view model {0} onto the back stack", viewModel?.GetType().Name ?? "null");
                    viewModelBackStack.Add(viewModel);
                    break;
            }
        }

        /// <inheritdoc/>
        protected override async void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content == null)
                return;

            CurrentParameter = e.Parameter;

            if (!(e.Content is Page view))
                throw new ArgumentException("View '" + e.Content.GetType().FullName +
                                            "' should inherit from Page or one of its descendants.");

            object cachedViewModel = null;

            switch (e.NavigationMode)
            {
                case NavigationMode.Back:
                    cachedViewModel = Pop(viewModelBackStack);
                    break;
                case NavigationMode.Forward:
                    cachedViewModel = Pop(viewModelForwardStack);
                    break;
            }

            await BindViewModel(view, cachedViewModel);
        }

        private static T Pop<T>(IList<T> stack)
        {
            if (stack.Count == 0)
                return default;

            var value = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return value;
        }
    }
}
