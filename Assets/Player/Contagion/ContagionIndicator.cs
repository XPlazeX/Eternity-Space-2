using UnityEngine;

public class ContagionIndicator : MonoBehaviour
{
    [SerializeField] private GameObject[] _togglingObjects;
    [SerializeField] private int _topValue = 11;

    private void OnEnable() {
        ContagionHandler.ContagionChanged += OnContagionChanged;
        OnContagionChanged();
    }

    private void OnDisable() {
        ContagionHandler.ContagionChanged -= OnContagionChanged;
    }

    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ContagionHandler.AddContagion(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ContagionHandler.RemoveContagion(1);
        }
    }
    #endif

    public void OnContagionChanged()
    {
        for (int i = 0; i < _togglingObjects.Length; i++)
        {
            if (i < _topValue)
            {
                _togglingObjects[i].SetActive(i < ContagionHandler.ContagionLevel);
            }
        }
    }
}
