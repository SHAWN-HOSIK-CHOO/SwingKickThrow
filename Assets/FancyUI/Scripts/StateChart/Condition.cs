using System.Collections.Generic;

namespace STC
{
    public class Condition : ICondition
    {
        public struct AlwaysTrue : ICondition
        {
            public void AddCondition(ICondition condition)
            {
            }

            public bool CheckCondition()
            {
                return true;
            }
        }

        List<ICondition> _conditionList;
        public Condition()
        {
            _conditionList = new List<ICondition>();
        }
        public void AddCondition(ICondition condition)
        {
            _conditionList.Add(condition);
        }
        public bool CheckCondition()
        {
            foreach (var condition in _conditionList)
            {
                if (!condition.CheckCondition())
                    return false;
            }
            return true;
        }
    }
}