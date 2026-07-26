using UnityEngine;

public class HideUISpawner : MonoBehaviour
{
    void Start()
    {
        Spawner spawner = SceneStatics.SceneCore.GetComponent<Spawner>();

        spawner.HideAll();
    }
}
