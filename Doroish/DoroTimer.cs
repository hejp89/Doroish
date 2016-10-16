using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Doroish {
    class DoroTimer {

        private DispatcherTimer Timer = new DispatcherTimer(), BreakTimer = new DispatcherTimer();
        private List<Doro> Doros;
        private int CurrentPosition;
        public bool IsRunning = false, IsBreak = false;
        private DateTime DoroStartTime, BreakStartTime;
        public delegate void TickDel(DoroTimerEvent e);
        public event TickDel Tick;

        public TimeSpan Elapsed { get { return DateTime.Now - DoroStartTime; } }
        public TimeSpan BreakElapsed { get { return DateTime.Now - BreakStartTime; } }
        public Doro CurrentDoro { get { return Doros[CurrentPosition]; } }

        public DoroTimer(List<Doro> doros) {
            Doros = doros;
        }

        public void Start() {
            if(Doros == null || Doros.Count == 0) {
                return;
            }

            CurrentPosition = 0;

            Timer.Tick += Timer_Tick;
            BreakTimer.Tick += BreakTimer_Tick;
            Timer.Interval = Doros[CurrentPosition].Duration;
            Timer.Start();

            DoroStartTime = DateTime.Now;

            Tick(new DoroTimerEvent(DoroTimerEvent.STARTED_DORO, Doros[CurrentPosition]));

            IsRunning = true;
        }

        public void Stop() {
            Timer.Stop();
            BreakTimer.Stop();

            IsRunning = false;
        }

        private void BreakTimer_Tick(object sender, object e) {
            BreakTimer.Stop();

            CurrentPosition++;

            Timer.Interval = Doros[CurrentPosition].Duration;
            Timer.Start();

            IsBreak = false;

            DoroStartTime = DateTime.Now;

            Tick(new DoroTimerEvent(DoroTimerEvent.STARTED_DORO, Doros[CurrentPosition]));
        }

        private void Timer_Tick(object sender, object e) {
            Timer.Stop();

            Tick(new DoroTimerEvent(DoroTimerEvent.FINISHED_DORO, Doros[CurrentPosition]));

            if(CurrentPosition + 1 == Doros.Count) {

                Tick(new DoroTimerEvent(DoroTimerEvent.FINISHED, null));
                IsRunning = false;

                return;
            }

            BreakStartTime = DateTime.Now;

            BreakTimer.Interval = Doros[CurrentPosition].BreakDuration;
            BreakTimer.Start();

            IsBreak = true;
        }    
    }

    public class DoroTimerEvent {
        public const string STARTED_DORO = "started doro", FINISHED_DORO = "finished doro", FINISHED = "finished";

        public string EventDescription;
        public Doro Doro;

        public DoroTimerEvent(string eventDescription, Doro doro) {
            EventDescription = eventDescription;
            Doro = doro;
        }
    }

}
