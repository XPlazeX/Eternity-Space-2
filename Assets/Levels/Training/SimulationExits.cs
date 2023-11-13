using UnityEngine;

public class SimulationExits : MonoBehaviour
{
    [SerializeField] private bool _noPauseExit = false;
    [SerializeField] private bool _noDeathDataErase = false;
    
    void Start()
    {
        if (_noPauseExit)
            VictoryHandler.CustomSceneOnExit = "Intro";

        if (_noDeathDataErase)
            DeathUIHandler.NoEraseData = true;

        VictoryHandler.CustomSceneOnDeath = "MissionMenu";

    }
}
