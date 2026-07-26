using UnityEngine;
using StatsManipulating;

public class EternityExecutor : MonoBehaviour
{
    [SerializeField] private StatOperator[] _statsOnEternityParse;

    private void Start() 
    {
        if (EternityClock.Parsing)
        {
            print("<color=magenta>ETERNITY PARSING</color>");
            for (int i = 0; i < _statsOnEternityParse.Length; i++)
            {
                _statsOnEternityParse[i].Enforce();
            }
        }
    }
}
