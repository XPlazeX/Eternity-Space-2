using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] private float _averageReload;
    [SerializeField] private float _minSpawnAngle = 150f;
    [SerializeField] private float _maxSpawnAngle = 210f;
    [SerializeField] private float _waitTime = 3f;
    [SerializeField] private EnvironmentDB _defaultDB;
    [SerializeField] private bool _autoStart = false;
    [Header("Информация только для просмотра. Изменения не будут учтены.")]
    [SerializeField] private AnimationCurve _selectionCurve;
    [SerializeField] private List<PullableObject> _objects = new List<PullableObject>();

    private List<PullForObjects> EnvironmentsPools = new List<PullForObjects>();
    private Dictionary<int, int> Codes = new Dictionary<int, int>();
    private float _maxTime = 0f;

    public float AverageReload {get; set;}
    public float WindAngle {get; set;} = 0f;

    private void Start() {
        if (_autoStart)
            Initialize();
    }

    public void Initialize() 
    {
        AverageReload = _averageReload;

        _selectionCurve = new AnimationCurve();
        AddEnvironmentDB(_defaultDB);

        StartCoroutine(Spawning());

        VictoryHandler.LevelVictored += OnLevelVictory;
    }

    private void OnDisable() {
        VictoryHandler.LevelVictored -= OnLevelVictory;
    }

    public void AddEnvironmentDB(EnvironmentDB edb)
    {
        for (int i = 0; i < edb.Objects.Length; i++)
        {
            _objects.Add(edb.Objects[i].Sample);
            _maxTime += edb.Objects[i].Chance;

            Keyframe k = new Keyframe();
            k.time = _maxTime;
            k.value = _objects.Count - 1;
            k.inTangent = 0f;
            k.outTangent = 0f;

            _selectionCurve.AddKey(k);
        }
        print("Success mod Environment Spawner!");
    }

    private IEnumerator Spawning()
    {
        yield return new WaitForSeconds(_waitTime);
        while (true)
        {
            SpawnObject(Mathf.RoundToInt(_selectionCurve.Evaluate(Random.Range(0f, _maxTime))));
            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(AverageReload));
        }
    }

    private void SpawnObject(int selectedIndex)
    {
        var environmentObject = GetEnvironmentObject(selectedIndex);

        environmentObject.transform.position =  Quaternion.Euler(0, 0, WindAngle) * (new Vector3(
            Random.Range(CameraController.Borders_xXyY.x - 2.5f, CameraController.Borders_xXyY.y + 2.5f),
            (CameraController.Borders_xXyY.w + 5f), 0f));

        environmentObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(_minSpawnAngle, _maxSpawnAngle) + WindAngle);
        environmentObject.SetActive(true);
    }

    private void OnLevelVictory()
    {
        StopAllCoroutines();
    }

    private GameObject GetEnvironmentObject(int selectedID)
    {
        if (!Codes.ContainsKey(selectedID))
        {
            Codes[selectedID] = EnvironmentsPools.Count;
            EnvironmentsPools.Add(new PullForObjects(_objects[selectedID]));
        }

        return EnvironmentsPools[Codes[selectedID]].GetGameObject(false);
    }
}
