using UnityEngine;
using UnityEngine.UI;

public class MissionStopper : MonoBehaviour
{
    [SerializeField] private TogglerButton[] _togButtons;
    [SerializeField] private Text _locationTerminal;

    private void Start() {
        for (int i = 0; i < _togButtons.Length; i++)
        {
            _togButtons[i].Toggled += CheckState;
        }
    }

    private void CheckState()
    {
        bool check = true;
        for (int i = 0; i < _togButtons.Length; i++)
        {
            if (!_togButtons[i].ON)
                check = false;
        }

        if (check)
            Reset();
    }

    private void Reset()
    {
        GameSessionInfoHandler.ClearGameSession();
        SceneTransition.SwitchToScene("Lobby");
    }
}
