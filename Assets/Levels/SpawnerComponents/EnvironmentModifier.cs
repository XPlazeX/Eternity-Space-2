using UnityEngine;

public class EnvironmentModifier : MonoBehaviour, TypeModifier
{
    [SerializeField] private float _averageReloadMultiplier;

    private void Start() {
        Enforce();
    }

    public void Enforce()
    {
        EnvironmentSpawner environmentSpawner = GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>();

        environmentSpawner.ReloadMultiplier *= _averageReloadMultiplier;
    }
}
