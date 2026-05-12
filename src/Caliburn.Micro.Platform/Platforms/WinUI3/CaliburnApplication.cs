using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Caliburn.Micro
{
    /// <summary>
    /// A base application class for WinUI3 that integrates with the Caliburn.Micro framework.
    /// Inherit from this instead of <see cref="Application"/> and override the relevant methods
    /// to configure your IoC container and select assemblies.
    /// </summary>
    public abstract class CaliburnApplication : Application
    {
        private bool isInitialized;

        /// <summary>
        /// The root frame of the application.
        /// </summary>
        protected Frame RootFrame { get; private set; }

        /// <summary>
        /// The main <see cref="Window"/> of the application, set by <see cref="InitializeWindow"/>.
        /// </summary>
        public Window Window { get; private set; }

        /// <summary>
        /// Called at design time to start the framework.
        /// </summary>
        protected virtual void StartDesignTime()
        {
            AssemblySource.Instance.Clear();
            AssemblySource.AddRange(SelectAssemblies());

            Configure();
            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        /// Called at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            AssemblySourceCache.Install();
            AssemblySource.AddRange(SelectAssemblies());

            PrepareApplication();
            Configure();

            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        /// Initializes the Caliburn.Micro framework. Call this before displaying any UI.
        /// </summary>
        protected void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            PlatformProvider.Current = new XamlPlatformProvider();

            var baseExtractTypes = AssemblySourceCache.ExtractTypes;

            AssemblySourceCache.ExtractTypes = assembly =>
            {
                var baseTypes = baseExtractTypes(assembly);
                var elementTypes = assembly.GetExportedTypes()
                    .Where(t => typeof(UIElement).IsAssignableFrom(t));
                return baseTypes.Union(elementTypes);
            };

            AssemblySource.Instance.Refresh();

            if (Execute.InDesignMode)
            {
                try { StartDesignTime(); }
                catch { isInitialized = false; throw; }
            }
            else
            {
                StartRuntime();
            }
        }

        /// <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Override to configure the framework and set up your IoC container.
        /// </summary>
        protected virtual void Configure() { }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
            => new[] { GetType().GetTypeInfo().Assembly };

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        protected virtual object GetInstance(Type service, string key)
            => System.Activator.CreateInstance(service);

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        protected virtual IEnumerable<object> GetAllInstances(Type service)
            => new[] { System.Activator.CreateInstance(service) };

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        protected virtual void BuildUp(object instance) { }

        /// <summary>
        /// Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        protected virtual void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) { }

        /// <summary>
        /// Creates a new <see cref="Window"/> for the application.
        /// </summary>
        protected virtual Window CreateWindow() => new Window();

        /// <summary>
        /// Initializes the main application window if not already created.
        /// </summary>
        public void InitializeWindow()
        {
            if (Window == null)
                Window = CreateWindow();
        }

        /// <summary>
        /// Creates the root frame used by the application.
        /// </summary>
        protected virtual Frame CreateApplicationFrame() => new Frame();

        /// <summary>
        /// Allows you to trigger the creation of the RootFrame from Configure if necessary.
        /// </summary>
        protected virtual void PrepareViewFirst()
        {
            if (RootFrame != null)
                return;

            RootFrame = CreateApplicationFrame();
            PrepareViewFirst(RootFrame);
        }

        /// <summary>
        /// Override this to register a navigation service.
        /// </summary>
        protected virtual void PrepareViewFirst(Frame rootFrame) { }

        /// <summary>
        /// Creates the root frame, navigates to the specified view and activates the window.
        /// </summary>
        protected void DisplayRootView(Type viewType, object parameter = null)
        {
            Initialize();
            InitializeWindow();
            PrepareViewFirst();

            RootFrame.Navigate(viewType, parameter);
            Window.Content = RootFrame;
            Window.Activate();
        }

        /// <summary>
        /// Creates the root frame, navigates to the specified view and activates the window.
        /// </summary>
        protected void DisplayRootView<T>(object parameter = null)
            => DisplayRootView(typeof(T), parameter);

        /// <summary>
        /// Resolves the view model, binds its view, activates it and shows it as the root view.
        /// </summary>
        protected async Task DisplayRootViewForAsync(Type viewModelType, CancellationToken cancellationToken)
        {
            Initialize();

            var viewModel = IoC.GetInstance(viewModelType, null);
            var view = ViewLocator.LocateForModel(viewModel, null, null);

            ViewModelBinder.Bind(viewModel, view, null);

            if (viewModel is IActivate activator)
                await activator.ActivateAsync(cancellationToken);

            InitializeWindow();
            Window.Content = view;
            Window.Activate();
        }

        /// <summary>
        /// Resolves the view model, binds its view, activates it and shows it as the root view.
        /// </summary>
        protected Task DisplayRootViewForAsync(Type viewModelType)
            => DisplayRootViewForAsync(viewModelType, CancellationToken.None);

        /// <summary>
        /// Resolves the view model, binds its view, activates it and shows it as the root view.
        /// </summary>
        protected Task DisplayRootViewForAsync<T>(CancellationToken cancellationToken)
            => DisplayRootViewForAsync(typeof(T), cancellationToken);

        /// <summary>
        /// Resolves the view model, binds its view, activates it and shows it as the root view.
        /// </summary>
        protected Task DisplayRootViewForAsync<T>()
            => DisplayRootViewForAsync<T>(CancellationToken.None);
    }
}
