using System;
using System.Collections.Generic;
using UnityEngine;
using static MVVM.Property.IProperty;

namespace MVVM.Property
{
    public interface IProperty
    {
        delegate void DelegateSubscribe();
        void Subscribe(DelegateSubscribe callback);
    }

    public class Property<T> : IProperty
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }
        public event Action<T> OnValueChanged;
        public void Subscribe(DelegateSubscribe callback)
        {
            OnValueChanged += (value) => callback?.Invoke();    
        }
    }
    public class IntProperty : Property<int> { }
    public class BoolProperty : Property<bool> { }
    public class FloatProperty : Property<bool> { }
}