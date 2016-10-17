using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundNotificationComponent {
    public sealed class ToastNotificationBackgroundTask : IBackgroundTask {
        public void Run(IBackgroundTaskInstance taskInstance) {
            //Inside here developer can retrieve and consume the pre-defined 
            //arguments and user inputs;
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            var arguments = details.UserInput;
            //var input = details.Input.Lookup("1");

            Debug.WriteLine("sdfsdf");

            File.Create("C:\\Users\\howard\\Desktop\\t.txt");


            // ...
        }
    }
}
