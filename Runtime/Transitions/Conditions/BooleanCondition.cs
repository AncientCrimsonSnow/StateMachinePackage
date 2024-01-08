using StateMachinePackage.Runtime.Transitions.Wrapper;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class BooleanCondition : Condition
    {
        private BooleanWrapper _booleanWrapper;

        public BooleanCondition(BooleanWrapper booleanWrapper) {
            _booleanWrapper = booleanWrapper;
            _booleanWrapper.RegisterOnValueChanged(newValue =>
            {
                TriggerConditionMeet();
            });
        }
    }
}