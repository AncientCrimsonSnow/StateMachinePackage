namespace StateMachinePackage.Runtime.Transitions
{
    public abstract class Condition
    {
        public delegate void ConditionMeetDelegate();
        protected event ConditionMeetDelegate ConditionMeet;
        internal void AddListener(ConditionMeetDelegate handler)
        {
            ConditionMeet += handler;
        }

        internal void RemoveListener(ConditionMeetDelegate handler)
        {
            ConditionMeet -= handler;
        }

        internal void RemoveAllListeners()
        {
            ConditionMeet = null;
        }

        protected void TriggerConditionMeet()
        {
            ConditionMeet?.Invoke();
        }
    }
}