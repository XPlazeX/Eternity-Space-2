using UnityEngine;

public class RegenOnEnemyKill : MonoBehaviour
{
    [SerializeField] private int _regenValue;

    private void OnEnable() {
        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().EnemyKilled += Regen;
    }

    private void OnDisable() {
        SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().EnemyKilled -= Regen;
    }

    private void Regen() => PlayerShipData.RegenerateHP(_regenValue);
}
