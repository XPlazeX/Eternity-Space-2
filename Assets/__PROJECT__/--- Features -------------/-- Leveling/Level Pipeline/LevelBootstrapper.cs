using UnityEngine;

public class LevelBootstrapper : MonoBehaviour
{
    [SerializeField] private ScenarioRunner scenarioRunner;
    [Space()]
    [SerializeField] private bool autoStartScenario = true;
    [SerializeField] private LevelAsset debugLevelAsset;

    void Start()
    {
        if (debugLevelAsset != null)
        {
            BootstrapLevel(debugLevelAsset);
        }
    }

    public void BootstrapLevel(LevelAsset levelAsset)
    {
        // Visual
        GameObject visualCore = Instantiate(levelAsset.levelVisualPrefab, Vector3.zero, Quaternion.identity);

        // Setup


        // Scenario
        scenarioRunner.SetScenarioAsset(levelAsset.scenarioAsset);

        if (autoStartScenario)
        {
            scenarioRunner.StartRunning();
        }

        // Audio
        GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().SetOSTDelayed(levelAsset.soundtrack, 3f);
    }
}
