using UnityEngine;

public class RamIndicators : MonoBehaviour
{
    [SerializeField] private GameObject[] _ramIndicators;

    private void Start() {
        for (int i = 0; i < _ramIndicators.Length; i++)
        {
            _ramIndicators[i].SetActive(i <= GameSessionInfoHandler.CurrentLevel);
        }
    }
}
