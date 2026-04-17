using UnityEngine;

public class RRBootstrap : MonoBehaviour
{
    [SerializeField] private RuntimeStatDefaultsAsset defaultsAsset;
    [SerializeField] private bool initializeOnAwake = true;

    private void Awake()
    {
        if (initializeOnAwake)
            RR.Initialize(defaultsAsset);
    }
}