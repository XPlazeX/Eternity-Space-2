using UnityEngine;

public class EnvironmentModifier : MonoBehaviour, TypeModifier
{
    [SerializeField] private float _averageReloadMultiplier;
    [SerializeField] private bool _disableSpawn;

    private void Start() {
        Enforce();
    }

    public void Enforce()
    {
        EnvironmentSpawner environmentSpawner = GameObject.FindWithTag("Level core").GetComponent<EnvironmentSpawner>();

        if (_disableSpawn)
        {
            environmentSpawner.Stop();
            return;
        }

        environmentSpawner.ReloadMultiplier *= _averageReloadMultiplier;
    }
}
