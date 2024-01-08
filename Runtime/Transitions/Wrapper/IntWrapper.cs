
namespace StateMachinePackage.Runtime.Transitions.Wrapper
{
    public class IntWrapper
    {
        private int _value;

        public delegate void ValueChangedDelegate(int newValue);
        private event ValueChangedDelegate ValueChanged;

        public void RegisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged += handler;
        }

        public void UnregisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged -= handler;
        }

        public IntWrapper(int initialValue)
        {
            _value = initialValue;
        }

        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(_value);
                }
            }
        }
    }
}