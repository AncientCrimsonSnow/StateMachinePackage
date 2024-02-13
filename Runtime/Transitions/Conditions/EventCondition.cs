
using System;
using UnityEngine.Events;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class EventCondition : Condition
    {
        public EventCondition(ref Action eventTriggerMethod)
        {
            eventTriggerMethod += TriggerConditionMeetWrapper;
        }

        public EventCondition(UnityEvent unityEvent) 
        {
            unityEvent.AddListener(TriggerConditionMeetWrapper);
        }

        private void TriggerConditionMeetWrapper()
        {
            TriggerConditionMeet();
        }
    }
}