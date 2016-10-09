using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Doroish {
    public sealed partial class DoroDialog : ContentDialog {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(DoroDialog), new PropertyMetadata(default(string)));

        public DoroDialog() {
            this.InitializeComponent();
        }

        public Doro Doro {
            get { return new Doro(titleTextBox.Text, durationTimePicker.Time, breakTimePicker.Time); }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }
    }
}
