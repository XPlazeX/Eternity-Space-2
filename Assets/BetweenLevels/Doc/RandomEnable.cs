using UnityEngine;

public class RandomEnable : MonoBehaviour
{
    [SerializeField] private bool _singleMode = false;
    [SerializeField][Range(0, 1f)] private float _singleChance;
    [Space()]
    [SerializeField] private GameObject[] _selectionGroup;

    private void OnEnable() 
    {
        if (_singleMode)
        {
            gameObject.SetActive(Random.value < _singleChance);
        }
        else
        {
            int select = Random.Range(0, _selectionGroup.Length);

            for (int i = 0; i < _selectionGroup.Length; i++)
            {
                _selectionGroup[i].SetActive(i == select);
            }
        }
    }
}
