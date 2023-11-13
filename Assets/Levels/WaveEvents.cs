using System.Collections.Generic;
using UnityEngine;

public class WaveEvents : MonoBehaviour
{
    [SerializeField] private List<WaveCondition> _conditionList = new List<WaveCondition>();

    protected EnemyDBSpawner _edbSpawner;
    protected virtual void Start() 
    {
        _edbSpawner = SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>();
        CheckConditions();

        _edbSpawner.WaveCompleted += CheckConditions;
    }

    private void OnDisable() {
        _edbSpawner.WaveCompleted -= CheckConditions;
    }

    public void CheckConditions()
    {
        int selectedID = -1;

        for (int i = 0; i < _conditionList.Count; i++)
        {
            if (_conditionList[i].CheckCondition(_edbSpawner.CurrentWave, _edbSpawner.WaveCount))
            {
                selectedID = i;
                _conditionList[i].Trigger();
                print($"secces cond: {i}, trig: {_conditionList[i].Triggered}");
                break;
            }
        }

        if (selectedID != -1)
            Trigger(selectedID);
    }

    protected virtual void Trigger(int conditionID){}

    [System.Serializable]
    private class WaveCondition
    {
        [SerializeField][Range(0, 100)] private int _targetWave;
        [SerializeField] private bool _byPercentage;
        [SerializeField][Range(0, 1f)] private float _targetPercentage;

        public bool Triggered {get; private set;}

        public void Trigger() => Triggered = true;

        public bool CheckCondition(int wave, int maxWave)
        {
            if (Triggered)
                return false;

            if (_byPercentage && ((float)wave / maxWave >= _targetPercentage))
                return true;

            else if (!_byPercentage && (wave == _targetWave))
                return true;

            return false;
        }
    }

}
