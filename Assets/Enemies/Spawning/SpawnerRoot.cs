using System.Collections;
using UnityEngine;

public class SpawnerRoot : MonoBehaviour
{
    public delegate void spawnerAction();
    public event spawnerAction SpawnerEnded;

    [Space()]
    [SerializeField] private float _startingTime;
    [SerializeField] protected float _reloadTime;

    protected Spawner _spawner;
    private bool _stopped = false;

    public virtual void StartSpawning()
    {
        // тело
        _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();

        if (_stopped)
            return;

        StartCoroutine(Spawning());
    }

    private void Start() {
        VictoryHandler.LevelVictored += Stop;
    }

    public virtual void Stop()
    {
        StopAllCoroutines();
        _stopped = true;
        print("СПАУНЕР ОСТАНОВЛЕН");
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= Stop;
    }

    protected virtual bool CheckConditions()
    {
        return false;
    }

    //protected virtual void NextCheck(){}

    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(_startingTime);

        while (CheckConditions())
        {
            yield return StartCoroutine(SpawnBody());
        }

        print("СПАУНЕР ЗАКОНЧИЛ");
        SpawnerEnded?.Invoke();
    }

    protected virtual IEnumerator SpawnBody() // корутина спауна. Можно использовать внутренние циклы, для каких-либо условий.
    {
        yield return null;
    }

}
