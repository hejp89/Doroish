using System.Collections.Generic;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Microsoft.QueryStringDotNET;

namespace Doroish {

    public sealed partial class MainPage : Page {
        public ObservableCollection<Doro> DoroList;

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

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            foreach(Doro doro in DoroList) {
                System.Diagnostics.Debug.WriteLine(doro.Title);
            }

            string title = "Andrew sent you a picture";

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
                    new ToastTextBox("tbReply") { PlaceholderContent = "Type a response" }
                },
                Buttons = {
                    new ToastButton("Reply", new QueryString() {
                        { "action", "reply" },
                        { "conversationId", conversationId.ToString() }
                    }.ToString()) {
                        ActivationType = ToastActivationType.Background,
                        TextBoxId = "tbReply"
                    },
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
