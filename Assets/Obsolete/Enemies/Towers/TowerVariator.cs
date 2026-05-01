using UnityEngine;

public class TowerVariator : MonoBehaviour
{
    [SerializeField] private GameObject[] _towerVariants;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _parentTransform;

    private void Start() {
        int id = Random.Range(0, _towerVariants.Length);

        Transform t = Instantiate(_towerVariants[id], _spawnPoint.position, Quaternion.identity).transform;

        t.SetParent(_parentTransform);
    }
}
