
using System;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class EventCondition : Condition
    {
        public EventCondition(ref Action eventTriggerMethod)
        {
            eventTriggerMethod += TriggerConditionMeetWrapper;
        }

        private void TriggerConditionMeetWrapper()
        {
            TriggerConditionMeet();
        }
    }
}