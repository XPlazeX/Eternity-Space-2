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
            if (dbs[i].GetComponent<EnemyHandle>() == null && dbs[i] is not PlayerDamageBody)
            {
                EnemySpawner.ValidatePrespawnedDamageBody(dbs[i]);
            }
        }

        PrespawnedValidated?.Invoke();
    }
}
