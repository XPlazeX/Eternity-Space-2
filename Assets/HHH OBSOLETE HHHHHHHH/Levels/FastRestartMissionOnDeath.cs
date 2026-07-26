using UnityEngine;

public class FastRestartMissionOnDeath : MonoBehaviour
{
    void Start()
    {
        VictoryHandler.EnableFastRestartOnDeath();
    }
}
