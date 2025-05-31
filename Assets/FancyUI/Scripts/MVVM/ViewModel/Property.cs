using STC;
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
        string Name { get; }
        T Value { get; set; }
        void SetSilently(T val);
    }
    public class Property<T> : IPropertySetter<T>, IPropertyNofier<T> 
    {
        bool initalized;
        private T _value;
        public string Name { get; private set; }
        

        
        public Property(string name = nameof(Property))
        {
            Name = name;
        }

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

        public void SetSilently(T val)
        {
            _value = val;
        }

        public void Subscribe(Action callback)
        {
            OnValueChanged += (value) => callback?.Invoke();
        }

    }
     public class IntProperty : Property<int> 
    { 
        public IntProperty(string name = nameof(IntProperty)) : base(name)
        {

        }
    }

    public class BoolProperty : Property<bool>
    {
        public BoolProperty(string name = nameof(BoolProperty)) : base(name)
        {

        }
    }

    public class FloatProperty : Property<bool>
    {
        public FloatProperty(string name = nameof(FloatProperty)) : base(name)
        {

        }

    }
}