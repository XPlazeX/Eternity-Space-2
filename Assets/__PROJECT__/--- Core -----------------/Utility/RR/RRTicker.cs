using UnityEngine;

public class RRTicker : MonoBehaviour
{
    [SerializeField] private bool useUnscaledTime = false;

    private void Update()
    {
        float dt = useUnscaledTime ? ESTime.unscaledDeltaTime : ESTime.worldDeltaTime;
        RR.Tick(dt);
    }
}