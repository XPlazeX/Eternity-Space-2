using UnityEngine;

public class CompositeModule : Module
{
    [SerializeField] private Module[] _loadingModules;

    public override void Asquiring()
    {
        for (int i = 0; i < _loadingModules.Length; i++)
        {
            _loadingModules[i].Asquiring();
        }
    }

    public override void Load()
    {
        for (int i = 0; i < _loadingModules.Length; i++)
        {
            ModuleCore.SpawnModule(_loadingModules[i]);
        }
    }
}
