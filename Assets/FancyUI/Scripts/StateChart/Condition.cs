using MVVM.Property;
using System;
using System.Collections.Generic;

namespace STC
{
    public class Condition : ICondition
    {
        public string Name { get; private set; }
        public Property<bool> IsActive { get;}
        List<ICondition> _conditionList;
        protected IProperty.DelegateSubscribe Callback;
        public event Action Dirty;
        public Condition(string name ,bool isActive)
        {
            Name = name;
            _conditionList = new List<ICondition>();
            IsActive = new Property<bool>();
            IsActive.Value = isActive;
            Callback = CheckDirty;
            IsActive.Subscribe(Callback);
        }

        public void CheckDirty()
        {
            if(CheckCondition())
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
            if (!IsActive.Value) return true;
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
}