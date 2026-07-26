using UnityEngine;

public class JoystickHUD : MonoBehaviour
{
    [SerializeField] private GameObject togglingObject;

    private void OnEnable() {
        SledgeDirector.PlayerCatchStarted += OnPlayerCatchingStarted;
        SledgeDirector.PlayerDeployed += OnPlayerDeployed;

        SledgeDirector sledgeDirector = GameObject.FindAnyObjectByType<SledgeDirector>();

        if (sledgeDirector == null)
        {
            togglingObject.SetActive(false);
            return;
        }

        togglingObject.SetActive(SledgeDirector.IsPlayerControlling);
    }

    void OnDisable()
    {
        SledgeDirector.PlayerCatchStarted -= OnPlayerCatchingStarted;
        SledgeDirector.PlayerDeployed -= OnPlayerDeployed;
    }

    private void OnPlayerCatchingStarted()
    {
        togglingObject.SetActive(false);
    }

    private void OnPlayerDeployed()
    {
        togglingObject.SetActive(true);
    }
}
