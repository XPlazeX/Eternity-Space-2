using UnityEngine;

public class Reloads : MonoBehaviour
{
    bool _charged = false;

    public void ReloadScene()
    {
        if (_charged)
            return;

        SceneTransition.ReloadScene();
        _charged = true;
    }

    public void GoToIntro()
    {
        if (_charged)
            return;
            
        SceneTransition.SwitchToScene("Intro");
        _charged = true;
    }
}
