using UnityEngine;

public class ClearSave : MonoBehaviour
{
    [SerializeField] private TogglerButton[] _togButtons;

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
        GlobalSaveHandler.ClearSave();
        int presetType = Mathf.RoundToInt(PlayerPrefs.GetFloat("ClearSavePreset", 0));

        switch (presetType)
        {
            case 0:
                print("Ручная очистка: стандартное сохранение");
                break;
            case 1:
                Unlocks.NewUnlock(5);
                TrainingOperator.PrepareSimulationTraining();
                print("Ручная очистка: криосон пропущен");
                break;
            case 2:
                Unlocks.NewUnlock(5);
                Unlocks.NewUnlock(6);
                GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().SetSessionData(2);
                print("Ручная очистка: обучение пройдено");
                break;
            default:
                break;
        }

        Invoke("Reload", 0.5f);
    }

    public void Reload()
    {
        SceneTransition.ReloadScene();
    }
}
