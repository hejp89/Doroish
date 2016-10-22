using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.QueryStringDotNET;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Windows.UI.Popups;

namespace Doroish {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;


        }

        /* This callback is trigged  */
        protected override async void OnActivated(IActivatedEventArgs e) {
            
            StorageFolder docs = await KnownFolders.DocumentsLibrary.CreateFolderAsync("Doroish", CreationCollisionOption.OpenIfExists);
            StorageFile output = await docs.CreateFileAsync(DateTime.Now.ToString("yyyy-MM-dd") + ".txt", CreationCollisionOption.OpenIfExists);

            if(e is ToastNotificationActivatedEventArgs) {
                var ev = e as ToastNotificationActivatedEventArgs;
                var args = QueryString.Parse(ev.Argument);

                if(!ev.UserInput.ContainsKey("tbNote")) {
                    return;
                }

                List<string> lines = new List<string>() { DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " - " + args["dorotitle"] + ":",
                                                          "",  ev.UserInput["tbNote"].ToString(), "", ""};
                await FileIO.AppendLinesAsync(output, lines);

                var configFile = await ApplicationData.Current.LocalFolder.GetFileAsync("config.json");
                if(configFile != null) {
                    try {
                        var jsonConfigString = await FileIO.ReadTextAsync(configFile);
                        JObject jsonConfig = JObject.Parse(jsonConfigString);

                        var requests = jsonConfig["requests"] as JArray;
                        foreach(var request in requests) {
                            if(request["method"].ToString() == "POST") {
                                using(var client = new HttpClient()) {

                                    var content = new StringContent(string.Format(request["body"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm"), args["dorotitle"], ev.UserInput["tbNote"].ToString()));

                                    var response = await client.PostAsync(request["body"].ToString(), content);

                                    var responseString = await response.Content.ReadAsStringAsync();
                                }
                            }

                            if(request["method"].ToString() == "GET") {
                                using(var client = new HttpClient()) {

                                    var url = string.Format(request["url"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm"), args["dorotitle"], ev.UserInput["tbNote"].ToString());

                                    var response = await client.GetAsync(url);

                                    var responseString = await response.Content.ReadAsStringAsync();
                                }
                            }
                        }
                    } catch {
                        var dialog = new MessageDialog("There was an error in the config.json file.");
                        await dialog.ShowAsync();
                    }
                }
            }

            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e) {
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached) {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if(rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if(e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if(e.PrelaunchActivated == false) {
                if(rootFrame.Content == null) {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }

            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder() {
                Name = "DoroToastTask",
                TaskEntryPoint = "BackgroundNotificationComponent.ToastNotificationBackgroundTask"
            };

            builder.SetTrigger(new ToastNotificationActionTrigger());

            BackgroundTaskRegistration registration = builder.Register();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// 
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
