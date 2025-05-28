using System;

namespace MVVM.Property
{
    public interface IPropertyNofier<T> 
    {
        void Subscribe(Action callback);
        void Invoke();
        T Get();
    }
    public interface IPropertySetter<T>
    {
        T Value { get; set; }
    }
    public class Property<T> : IPropertySetter<T>, IPropertyNofier<T> 
    {
        bool initalized;
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value) || !initalized)
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                    initalized = true;
                }
            }
        }
        public event Action<T> OnValueChanged;
        public T Get()
        {
            return Value;
        }
        public void Invoke()
        {
            OnValueChanged?.Invoke(Value);
        }

        public void Subscribe(Action callback)
        {
            OnValueChanged += (value) => callback?.Invoke();
        }
    }
     public class IntProperty : Property<int> { }

    public class BoolProperty : Property<bool> { }

    public class FloatProperty : Property<bool> { }
}