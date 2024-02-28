
using System;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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

#if ENABLE_INPUT_SYSTEM
        public EventCondition(InputAction inputAction)
        {
            inputAction.performed += ctx => TriggerConditionMeetWrapper();
        }
#endif

        private void TriggerConditionMeetWrapper()
        {
            TriggerConditionMeet();
        }
    }
}