
using StateMachinePackage.Runtime.Transitions.Wrapper;

namespace StateMachinePackage.Runtime.Transitions.Conditions
{
    public class FloatCondition : Condition
    {
        private float _targetValue;
        private ComparisonType _comparisonType;
        private FloatWrapper _floatWrapper;

        public FloatCondition(FloatWrapper floatWrapper, float targetValue, ComparisonType comparisonType)
        {
            _floatWrapper = floatWrapper;
            _targetValue = targetValue;
            _comparisonType = comparisonType;

            _floatWrapper.RegisterOnValueChanged(CheckCondition);
        }

        private void CheckCondition(float newValue)
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