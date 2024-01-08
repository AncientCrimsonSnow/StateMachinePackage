using StateMachinePackage.Runtime.Transitions.Wrapper;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class IntCondition : Condition
    {
        private int _targetValue;
        private ComparisonType _comparisonType;
        private IntWrapper _intWrapper;

        public IntCondition(IntWrapper intWrapper, int targetValue, ComparisonType comparisonType)
        {
            _intWrapper = intWrapper;
            _targetValue = targetValue;
            _comparisonType = comparisonType;

            _intWrapper.RegisterOnValueChanged(CheckCondition);
        }

        private void CheckCondition(int newValue)
        {
            bool conditionMet = false;

            switch (_comparisonType)
            {
                case ComparisonType.Equal:
                    conditionMet = newValue == _targetValue;
                    break;
                case ComparisonType.LessThan:
                    conditionMet = newValue < _targetValue;
                    break;
                case ComparisonType.GreaterThan:
                    conditionMet = newValue > _targetValue;
                    break;
                case ComparisonType.LessThanOrEqual:
                    conditionMet = newValue <= _targetValue;
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    conditionMet = newValue >= _targetValue;
                    break;
            }

            if (conditionMet)
            {
                TriggerConditionMeet();
            }
        }
    }
}