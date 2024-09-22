using UnityEngine;
using StatsManipulating;

public class EternitySpan : MonoBehaviour
{
    [SerializeField] private StatOperator[] _statsOnSpan;

    private void Start() {
        if (GlobalSaveHandler.GetSave().EternityParse)
        {
            for (int i = 0; i < _statsOnSpan.Length; i++)
            {
                _statsOnSpan[i].Enforce();
            }
        }
    }
}
