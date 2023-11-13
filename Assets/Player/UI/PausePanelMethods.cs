using UnityEngine;

public class PausePanelMethods : MonoBehaviour
{
    public delegate void levelEvent();

    public static event levelEvent SavedAndExited;

    [SerializeField] private GameObject _panel;

    public void SaveAndExit()
    {
        _panel.SetActive(false);
        //GameSessionInfoHandler.SaveAll();
        //SceneStatics.UICore.GetComponent<DeathUIHandler>().EndLevel();

        SavedAndExited?.Invoke();
        VictoryHandler.ExitSession();
    }
}
