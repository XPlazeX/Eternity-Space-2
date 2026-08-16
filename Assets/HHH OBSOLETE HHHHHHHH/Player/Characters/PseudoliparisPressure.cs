using UnityEngine;

public class PseudoliparisPressure : MonoBehaviour
{
    [SerializeField] private float _waitTime;
    [SerializeField] private float _loseReload;

    private float _timer;
    private int _negative = 0;

    private void Start() {
        _timer = _waitTime;
    }

    private void OnEnable() {
        VictoryHandler.LevelVictored += OnLevelVictoried;
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= OnLevelVictoried;
    }

    private void Update() 
    {
        _timer -= ESTime.worldDeltaTime;

        if (_timer <= 0 && _negative != 1)
        {
            if (_negative != -1)
            {
                AbyssDamage ad = GameObject.FindObjectOfType<AbyssDamage>();
                if (ad != null)
                {
                    _negative = 1;
                    ad.StopPressure();
                    return;
                }
                else
                {
                    _negative = -1;
                }
            }

            // PlayerShipData.ConsumeHP(1);
            _timer = _loseReload;
        }
    }

    private void OnLevelVictoried()
    {
        _negative = 1;
    }
}
