using UnityEngine;
using System.Collections;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private bool _test;
    [SerializeField] private DamageBody _testEnemy;
    [SerializeField] private int _count;
    [SerializeField] private WeightSelector _testWS;

    private void Start() {
        if (!_test)
            return;

        GetComponent<EnemyDBSpawner>().Stop();
        GetComponent<BonusSpawner>().Stop();

        StartCoroutine(Spawning());
    }

    private void OnEnable() {
        if (_testWS != null)
        {
            GetComponent<EnemyDBSpawner>().SetCustomWeightSelector(_testWS);
        }
    }

    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(4f);

        for (int i = 0; i < _count; i++)
        {
            Spawner.InitializeHPBar(Spawner.SpawnDamageBody(_testEnemy));
        }
    }
}
