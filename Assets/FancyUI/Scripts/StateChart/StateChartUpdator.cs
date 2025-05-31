using STC;
using UnityEngine;

public class StateChartUpdator : MonoBehaviour
{
    void Update()
    {
        StateChart.Instance.Update();
    }
}
