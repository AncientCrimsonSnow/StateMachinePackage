

using StateMachinePackage.Runtime.Transitions.Wrapper;
using System.Timers;
using UnityEngine;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class TimeCondition : Condition
    {
        private Timer _timer;
        private bool _timerRunning = false;

        public TimeCondition(FloatWrapper seconds)
        {
            if(seconds.Value > 0)
            {
                _timer = new Timer(seconds.Value * 1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = false;
            }
            else
                Debug.LogWarning("Timer cant have a 0 seconds interval");

            seconds.RegisterOnValueChanged(OnSecondsValueChanged);
        }

        public void StartTimer()
        {
            _timer.Start();
            _timerRunning = true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TriggerConditionMeetWrapper();
            _timerRunning = false;
        }

        private void TriggerConditionMeetWrapper()
        {
            TriggerConditionMeet();
        }

        private void OnSecondsValueChanged(float seconds)
        {
            if (seconds <= 0)
            {
                Debug.LogWarning("Timer cant have a 0 seconds interval");
                return;
            }

            if(_timer == null)
            {
                _timer = new Timer(seconds * 1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = false;
            }
            else
            {
                _timer.Stop();
                _timer.Interval = seconds * 1000;
                if (_timerRunning) _timer.Start();
            }

        }
    }
}