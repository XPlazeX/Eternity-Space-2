using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class EnemyHPBarInitializer : MonoBehaviour
{
    private void Start() 
    {
        Spawner.InitializeHPBar(GetComponent<DamageBody>());
    }
}
