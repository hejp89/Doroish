using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Microsoft.QueryStringDotNET;
using System.Linq;

namespace Doroish {

    public sealed partial class MainPage : Page {
        public ObservableCollection<Doro> DoroList;
        private DoroTimer DoroTimer;
        private DispatcherTimer UITimer = new DispatcherTimer();
        

        public MainPage() {
            InitializeComponent();
            DoroList = new ObservableCollection<Doro>();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e) {

            var dialog = new DoroDialog();
            var result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary) {
                var doro = dialog.Doro;
                DoroList.Add(doro);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            if(DoroListView.SelectedIndex == -1)
                return;
            DoroList.RemoveAt(DoroListView.SelectedIndex);
        }

        private void DoroTimer_Tick(DoroTimerEvent e) {
            if(e.EventDescription == DoroTimerEvent.FINISHED_DORO) {
                ShowNotification(e.Doro);
            }

            if(e.EventDescription == DoroTimerEvent.FINISHED) {
                UITimer.Stop();

                StatusBar.Text = "Finished";
                
                AddButton.IsEnabled = true;
                RemoveButton.IsEnabled = true;
                StartButton.Icon = new SymbolIcon(Symbol.Play);
            }
        }

        private void UITimer_Tick(object sender, object e) {
            var CurrentDoro = DoroTimer.CurrentDoro;

            if (DoroTimer.IsBreak) {
                var timeElapsed = DoroTimer.BreakElapsed;

                StatusBar.Text = string.Format("{0}: {1:00}:{2:00} / {3:00}:{4:00}", "Break", timeElapsed.Minutes, timeElapsed.Seconds, CurrentDoro.BreakDuration.Minutes, CurrentDoro.BreakDuration.Seconds);
            } else {
                var timeElapsed = DoroTimer.Elapsed;

                StatusBar.Text = string.Format("{0}: {1:00}:{2:00} / {3:00}:{4:00}", CurrentDoro.Title, timeElapsed.Minutes, timeElapsed.Seconds, CurrentDoro.Duration.Minutes, CurrentDoro.Duration.Seconds);
            }
        }



        private void StartButton_Click(object sender, RoutedEventArgs e) {

            if(DoroList.Count == 0) {
                return;
            }

            if(DoroTimer == null || !DoroTimer.IsRunning) {
                DoroTimer = new DoroTimer(DoroList.ToList());

                DoroTimer.Tick += DoroTimer_Tick;

                DoroTimer.Start();
                UITimer.Start();

                UITimer.Tick += UITimer_Tick;
                UITimer.Interval = new TimeSpan(0, 0, 1);
                UITimer.Start();

                AddButton.IsEnabled = false;
                RemoveButton.IsEnabled = false;
                StartButton.Icon = new SymbolIcon(Symbol.Stop);

                UITimer_Tick(null, null);
            } else {
                DoroTimer.Stop();
                UITimer.Stop();

                StatusBar.Text = "Stopped";

                AddButton.IsEnabled = true;
                RemoveButton.IsEnabled = true;
                StartButton.Icon = new SymbolIcon(Symbol.Play);
            }

        }

        private void ShowNotification(Doro doro) {
            string title = doro.Title + " has finished";

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual() {
                BindingGeneric = new ToastBindingGeneric() {
                    Children = {
                        new AdaptiveText() { Text = title }
                    }
                }
            };

            int conversationId = 384928;

            // Construct the actions for the toast (inputs and buttons)
            ToastActionsCustom actions = new ToastActionsCustom() {
                Inputs = {
                    new ToastTextBox("tbNote") { PlaceholderContent = "Add a note" }
                },
                Buttons = {
                    new ToastButton("Add Note", new QueryString() {
                        { "action", "reply" },
                        { "conversationId", conversationId.ToString() }
                    }.ToString()) {
                        ActivationType = ToastActivationType.Foreground,
                        TextBoxId = "tbNote"
                    }
                }
            };

            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent() {
                Visual = visual,
                Actions = actions,
                Launch = new QueryString() {
                    { "action", "viewConversation" },
                    { "conversationId", conversationId.ToString() }
                }.ToString()
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());

            toast.ExpirationTime = DateTime.Now.AddDays(2);

            toast.Tag = "1";
            toast.Group = "doro";

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
