using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using JellyBox.Views;
using Microsoft.Extensions.DependencyInjection;
using Jellyfin.Sdk;
using JellyBox.Services;
using JellyBox.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Core;

namespace JellyBox
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // Disable Xbox mouse cursor
            this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;

            // Disable default scaling
            bool result = Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(true);

            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.Navigated += OnNavigated;
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Ioc.Default.ConfigureServices(ConfigureServices(rootFrame));

                // Initialize the sdk client settings. This only needs to happen once on startup.
                var sdkClientSettings = Ioc.Default.GetRequiredService<SdkClientSettings>();
                sdkClientSettings.InitializeClientSettings(
                    "JellyBox",
                    "0.1.0",
                    "Sample Device",
                    $"this-is-my-device-id-{Guid.NewGuid():N}");

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(HomePage), e.Arguments);
                }
                // Bind back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when the frame is navigated on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Invoked when the back button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private static ServiceProvider ConfigureServices(Frame frame)
        {
            var serviceCollection = new ServiceCollection();

            static HttpMessageHandler DefaultHttpClientHandlerDelegate(IServiceProvider service)
                => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate,
                    //RequestHeaderEncodingSelector = (_, a) => Encoding.UTF8
                };

            // Add NavigationService
            serviceCollection
                .AddSingleton<INavigationService>(new NavigationService(frame));

            // Add SettingService
            serviceCollection.AddSingleton<SettingService>();

            // Add Http Client
            serviceCollection.AddHttpClient("Default", c =>
            {
                c.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue(
                        "JellyBox",
                        "0.1.0"));

                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json", 1.0));
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            })
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);

            // Add Jellyfin SDK services.
            serviceCollection
                .AddSingleton<SdkClientSettings>();
            serviceCollection
                .AddHttpClient<IDynamicHlsClient, DynamicHlsClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);
            serviceCollection
                .AddHttpClient<IItemsClient, ItemsClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);
            serviceCollection
                .AddHttpClient<ISystemClient, SystemClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);
            serviceCollection
                .AddHttpClient<IUserClient, UserClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);
            serviceCollection
                .AddHttpClient<IUserViewsClient, UserViewsClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);
            serviceCollection
                .AddHttpClient<IUserLibraryClient, UserLibraryClient>()
                .ConfigurePrimaryHttpMessageHandler(DefaultHttpClientHandlerDelegate);

            // Add Jellyfin service
            serviceCollection.AddSingleton<JellyfinService>();


            // Add ViewModels
            serviceCollection.AddTransient<HomePageViewModel>();
            serviceCollection.AddTransient<PlayerPageViewModel>();


            return serviceCollection.BuildServiceProvider();
        }
    }
}
