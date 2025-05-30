using MVVM.Property;
using MVVM.ViewModel.Window;
using System;
using System.Collections.Generic;

namespace STC
{
    public class Condition : ICondition
    {
        public string Name { get; private set; }
        public Property<bool> IsActive { get; }
        List<ICondition> _conditionList;
        public event Action Dirty;
        public Condition(string name, bool isActive)
        {
            Name = name;
            _conditionList = new List<ICondition>();
            IsActive = new Property<bool>();
            IsActive.Value = isActive;
            IsActive.Subscribe(CheckDirty);
        }

        public void CheckDirty()
        {
            if (CheckCondition())
            {
                Dirty?.Invoke();
            }
        }
        public void AddCondition(ICondition condition)
        {
            _conditionList.Add(condition);
        }
        public void RemoveCondition(ICondition condition)
        {
            _conditionList.Remove(condition);
        }
        public bool CheckCondition()
        {
            if (IsActive.Value == false) return true;
            foreach (var condition in _conditionList)
            {
                if (!condition.CheckCondition())
                    return false;
            }
            return OnCheck();
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
            Property.Subscribe(CheckDirty);
        }
        public override bool OnCheck()
        {
            return Property.Get() != Compare;
        }
    }
}