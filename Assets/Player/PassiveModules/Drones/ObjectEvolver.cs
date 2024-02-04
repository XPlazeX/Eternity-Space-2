using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEvolver : MonoBehaviour
{
    [Header("Объект появится на позиции игрока")]
    [SerializeField] private float _enforceDelay = 1f;
    [SerializeField] private string _keyName;
    [SerializeField] private GameObject[] _scalingGameObjects;

    private static Dictionary<string, int> EvolvingObjects = new Dictionary<string, int>();

    private void Start() {
        if (!EvolvingObjects.ContainsKey(_keyName))
        {
            EvolvingObjects[_keyName] = -1;
            StartCoroutine(EnforceCountdown());
        }

        EvolvingObjects[_keyName] ++;
    }

    private IEnumerator EnforceCountdown()
    {
        yield return new WaitForSeconds(_enforceDelay);

        int stage = EvolvingObjects[_keyName];
        if (stage >= _scalingGameObjects.Length)
            stage = _scalingGameObjects.Length - 1;

        Instantiate(_scalingGameObjects[stage], Player.PlayerTransform.position, Quaternion.identity);
    }
}
