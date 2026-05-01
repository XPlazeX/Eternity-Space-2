using UnityEngine;

[CreateAssetMenu(fileName = "LevelAsset", menuName = "LevelAsset", order = 0)]
public class LevelAsset : ScriptableObject 
{
    [Header("General")]
    public string id;
    [Range(0, 3)] public int actIndex = 0;
    public int levelIndex = 0;

    [Header("Visual")]
    public GameObject levelVisualPrefab;

    [Header("Setup")]
    // setup object

    [Header("Scenario")]
    public ScenarioAsset scenarioAsset;

    [Header("Audio")]
    public SoundObject soundtrack;
}