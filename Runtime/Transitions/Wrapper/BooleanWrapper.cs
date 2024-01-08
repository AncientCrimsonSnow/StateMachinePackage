using System;
using UnityEngine;

namespace StateMachinePackage.Runtime.Transitions.Wrapper
{
    [Serializable]
    public class BooleanWrapper
    {
        [SerializeField]
        private bool _value;

        public delegate void ValueChangedDelegate(bool newValue);

        private event ValueChangedDelegate ValueChanged;

        public void RegisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged += handler;
        }

        public void UnregisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged -= handler;
        }

        public BooleanWrapper(bool initialValue)
        {
            _value = initialValue;
        }

        public bool Value
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