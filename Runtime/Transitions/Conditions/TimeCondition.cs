

using System.Timers;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class TimeCondition : Condition
    {
        private Timer _timer;

        public TimeCondition(float seconds)
        {
            _timer = new Timer(seconds * 1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TriggerConditionMeetWrapper();
        }

        private void TriggerConditionMeetWrapper()
        {
            TriggerConditionMeet();
        }
    }
}