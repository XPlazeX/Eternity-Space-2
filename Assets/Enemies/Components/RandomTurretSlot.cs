using System.Collections;
using UnityEngine;

public class RandomTurretSlot : MonoBehaviour
{
    [SerializeField] private TurretStaticAI[] _turrets;

    private void Start() {
        Instantiate(_turrets[Random.Range(0, _turrets.Length)], transform.position, transform.rotation).transform.SetParent(transform);
    }
}
