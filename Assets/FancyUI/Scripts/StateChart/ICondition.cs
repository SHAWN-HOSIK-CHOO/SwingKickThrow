using UnityEngine;

namespace STC
{
    public interface ICondition
    {
        bool CheckCondition();
        void AddCondition(ICondition condition);
    }
}