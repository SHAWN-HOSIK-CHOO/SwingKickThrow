using MVVM.Property;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

namespace STC
{
    public class Condition : ICondition
    {
        public string Name { get; private set; }
        public BoolProperty IsValid { get; }

        List<ICondition> _ConditionList;


        public Condition(string name, bool isActive)
        {
            Name = name;
            _ConditionList = new List<ICondition>();
            IsValid = new BoolProperty();
            IsValid.Value = false;
        }
        public bool CheckValid()
        {
            bool valid = true;
            foreach (var condition in _ConditionList)
            {
                if (!condition.CheckValid())
                {
                    valid = false;
                    break;
                }
            }
            valid &= OnCheck();
            return valid;
        }


        public ICondition AddCondition(ICondition condition)
        {
            _ConditionList.Add(condition);
            condition.IsValid.Subscribe(OnSignal);
            return this;
        }

        public virtual void OnSignal(bool val)
        {
            if(val)
            {
                bool isvalid = CheckValid();
                IsValid.SetSilently(isvalid);
                if(isvalid)
                {
                   IsValid?.Invoke();
                }
            }
        }

        public virtual bool OnCheck()
        {
            return true;
        }
    }

    public class BoolBarrier : Condition
    {
        IPropertyNofier<bool> Property;
        bool Compare;
        public BoolBarrier(IPropertyNofier<bool> property, bool compare ,string name = nameof(BoolBarrier)) : base(name, true)
        {
            Property = property;
            Compare = compare;
            Property.Subscribe(OnSignal);
        }

        public override void OnSignal(bool val)
        {
            bool isvalid = CheckValid();
            IsValid.SetSilently(isvalid);
            if (isvalid)
            {
                IsValid?.Invoke();
            }
        }

        public override bool OnCheck()
        {
            return Property.Get() != Compare;
        }
    }
}