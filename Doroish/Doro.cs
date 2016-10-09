using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doroish {
    
    public class Doro {
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public string DurationString { get { return "Duration: " + FormatTimeSpan(Duration); } }
        public TimeSpan BreakDuration { get; set; }
        public string BreakDurationString { get { return "Break: " + FormatTimeSpan(BreakDuration); } }

        public Doro(string title, TimeSpan duration, TimeSpan breakDuration) {
            Title = title;
            Duration = duration;
            BreakDuration = breakDuration;
        }

        private string FormatTimeSpan(TimeSpan ts) {
            string result = "";
            if(ts.Hours == 1) {
                result += ts.Hours + "hr";
            }
            if(ts.Hours > 1) {
                result += ts.Hours + "hrs";
            }

            if(ts.Minutes == 1) {
                result += ts.Minutes + "min";
            }
            if(ts.Minutes > 1) {
                result += ts.Minutes + "mins";
            }

            return result;
        }

    }
}
