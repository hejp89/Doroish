using System.Collections.Generic;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;

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
        }
    }
}
