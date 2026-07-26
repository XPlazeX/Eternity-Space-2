using System.Collections;
using UnityEngine;

public class ObjectPreLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectsForPreLoad;

    private void Start() 
    {
        StartCoroutine(PreLoad());
    }

    private IEnumerator PreLoad()
    {
        for (int i = 0; i < _objectsForPreLoad.Length; i++)
        {
            _objectsForPreLoad[i].SetActive(true);
        }

        yield return null;

        for (int i = 0; i < _objectsForPreLoad.Length; i++)
        {
            _objectsForPreLoad[i].SetActive(false);
        }

        Destroy(this);
    }
}
