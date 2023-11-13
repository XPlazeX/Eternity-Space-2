using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetData : MonoBehaviour
{
    [SerializeField] private Image[] _buttons;
    [SerializeField] private Sprite _enabledSprite;
    [SerializeField] private Sprite _disabledSprite;

    private List<int> activeCodes = new List<int>();

    public void ToggleSwitch(int id)
    {
        if (activeCodes.Contains(id))
        {
            _buttons[id].sprite = _disabledSprite;
            activeCodes.Remove(id);
        }
        else
        {
            _buttons[id].sprite = _enabledSprite;
            activeCodes.Add(id);

            //GameObject.FindWithTag("SceneCore").GetComponent<MainSettings>().SetLogState(5);
            Check();
        }
    }

    private void Check()
    {
        if (activeCodes.Count != 4)
            return;

        GameSessionInfoHandler.ClearGameSession();
        PlayerPrefs.DeleteAll();

        SceneTransition.ReloadScene();
    }
}
