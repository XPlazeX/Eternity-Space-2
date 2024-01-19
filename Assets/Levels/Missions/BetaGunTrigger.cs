using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BetaGunTrigger : MonoBehaviour
{
    [SerializeField] private int _targetLevel;
    [SerializeField] private int _targetWave;

    private WaveSpawningMission _missionCore;

    private void OnEnable() {
        _missionCore = GameObject.FindObjectOfType<WaveSpawningMission>();
        _missionCore.OnNewWaveStarted.AddListener(TryTrigger);
    }

    private void OnDisable() {
        _missionCore.OnNewWaveStarted.RemoveListener(TryTrigger);
    }

    public void TryTrigger()
    {
        if (GameSessionInfoHandler.CurrentLevel == _targetLevel && SceneStatics.SceneCore.GetComponent<EnemyDBSpawner>().CurrentWave == _targetWave)
            GetComponent<Animator>().SetTrigger("Fire");
    }
}
