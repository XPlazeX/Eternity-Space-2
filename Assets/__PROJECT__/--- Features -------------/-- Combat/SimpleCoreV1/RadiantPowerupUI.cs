using UnityEngine;

public class RadiantPowerupUI : MonoBehaviour
{
    [SerializeField] private GameObject[] togglingObjects;

    private void OnEnable() 
    {
        SimpleCoreV1.RadiantLevelChanged += OnRadiantLevelChanged;
    }

    void OnDisable()
    {
        SimpleCoreV1.RadiantLevelChanged -= OnRadiantLevelChanged;
    }

    private void OnRadiantLevelChanged(int newLevel)
    {
        for (int i = 0; i < togglingObjects.Length; i++)
        {
            togglingObjects[i].SetActive(i < newLevel + 1);
        }
    }
}
