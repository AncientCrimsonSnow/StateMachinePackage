using System;
using UnityEngine;

namespace StateMachinePackage.Runtime.Transitions.Wrapper
{
    [Serializable]
    public class FloatWrapper
    {
        [SerializeField]
        private float _value;

        public delegate void ValueChangedDelegate(float newValue);
        private event ValueChangedDelegate ValueChanged;

        public void RegisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged += handler;
        }

        public void UnregisterOnValueChanged(ValueChangedDelegate handler)
        {
            ValueChanged -= handler;
        }

        public FloatWrapper(float initialValue)
        {
            _value = initialValue;
        }

        public float Value
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