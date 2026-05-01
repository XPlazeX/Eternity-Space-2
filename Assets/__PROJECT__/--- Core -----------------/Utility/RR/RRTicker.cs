using UnityEngine;

public class RRTicker : MonoBehaviour
{
    [SerializeField] private bool useUnscaledTime = false;

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        RR.Tick(dt);
    }
}