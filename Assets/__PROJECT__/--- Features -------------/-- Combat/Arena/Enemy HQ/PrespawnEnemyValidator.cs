using UnityEngine;

public class PrespawnEnemyValidator : MonoBehaviour
{
    public static event System.Action PrespawnedValidated;

    private void Start() {
        ValidatePrespawnedEnemies();
    }

    private void ValidatePrespawnedEnemies()
    {
        DamageBody[] dbs = FindObjectsByType<DamageBody>(FindObjectsSortMode.None);

        for (int i = 0; i < dbs.Length; i++)
        {
            if (dbs[i] is SledgeBody || dbs[i] is PlayerDamageBody) continue;

            if (dbs[i].GetComponent<EnemyHandle>() == null)
            {
                EnemySpawner.ValidatePrespawnedDamageBody(dbs[i]);
            }
        }

        PrespawnedValidated?.Invoke();
    }
}
