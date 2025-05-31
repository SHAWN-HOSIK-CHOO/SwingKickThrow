using MVVM.Property;
using MVVM.ViewModel.Window;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

namespace STC
{
    public class Condition : ICondition
    {
        public string Name { get; private set; }
       
        public BoolProperty IsActive { get; }
        public BoolProperty IsValid { get; }

        List<ICondition> _ConditionList;


        public Condition(string name, bool isActive)
        {
            Name = name;
            _ConditionList = new List<ICondition>();
            IsActive = new BoolProperty();
            IsValid = new BoolProperty();
            IsValid.Value = false;
            IsActive.Value = isActive;
            IsActive.Subscribe(()=>CheckValid(null));
        }

        public void CheckValid(ICondition except)
        {
            bool valid = true;
            if (IsActive.Value)
            {
                foreach (var condition in _ConditionList)
                {
                    if (!Equals(condition, except))
                    {
                        condition.CheckValid(except);
                    }
                    if (!condition.IsValid.Value)
                    {
                        valid = false;
                        break;
                    }
                }
                valid &= OnCheck();
            }
            IsValid.Value = valid;
        }
        public ICondition AddCondition(ICondition condition)
        {
            _ConditionList.Add(condition);
            condition.IsValid.Subscribe(()=>CheckValid(condition));
            return this;
        }

        public virtual bool OnCheck()
        {
            return true;
        }
    }

    public class ActionCondition : Condition
    {
        Func<bool> CheckFunc;
        bool Compare;
        public ActionCondition(Func<bool> func, bool compare ,string name = nameof(BoolBarrier)) : base(name, true)
        {
            CheckFunc = func;
            Compare = compare;
        }
        public override bool OnCheck()
        {
            return CheckFunc() == Compare;
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
            Property.Subscribe(()=> CheckValid(null));
        }
        public override bool OnCheck()
        {
            return Property.Get() != Compare;
        }
    }
}