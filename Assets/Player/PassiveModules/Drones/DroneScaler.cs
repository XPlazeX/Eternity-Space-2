using UnityEngine;

public class DroneScaler : MonoBehaviour
{
    [Header("Работает с IAttackModule")]
    [SerializeField] private bool _scaleLocalAggro;
    [SerializeField] private float _aggroBoostPerStep = 0.4f;
    [Space()]
    [Header("Работает с DamageBody")]
    [SerializeField] private bool _scaleHP;
    [SerializeField] private float _hpNormalizedBoostPerStep = 0.3f;
    [Space()]
    [Header("Работает с EnemyAIRoot")]
    [SerializeField] private bool _scaleLocalMobility;
    [SerializeField] private float _mobilityBoostPerStep = 0.2f;

    private void Start() 
    {
        float boostMultiplier = ShipStats.GetValue("DroneEffeciency") - 1f;

        if (boostMultiplier < 0f)
            boostMultiplier = 0f;

        if (_scaleLocalAggro)
        {
            GetComponent<IAttackModule>().LocalMultiplyAggro(1f + (_aggroBoostPerStep * boostMultiplier));
        }    

        if (_scaleHP)
        {
            GetComponent<DamageBody>().MultiplyHP(1f + (_hpNormalizedBoostPerStep * boostMultiplier));
        }

        if (_scaleLocalMobility)
        {
            GetComponent<EnemyAIRoot>().LocalMultiplyMobility(1f + (_mobilityBoostPerStep * boostMultiplier));
        }
    }

}
