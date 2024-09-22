using UnityEngine;
using UnityEngine.UI;

public class DevToggle : MonoBehaviour
{
    [SerializeField] private GameObject[] _togglingObjects;
    [SerializeField] private Text _allUnlockLabel;

    public static bool AllUnlocked {get; private set;} = false;

    private void Start() {
        for (int i = 0; i < _togglingObjects.Length; i++)
        {
            _togglingObjects[i].SetActive(Dev.DevVersion);
        }

        if (_allUnlockLabel != null)
        {
            AllUnlocked = false;
            _allUnlockLabel.text = SceneLocalizator.GetLocalizedString("Intro", 6, AllUnlocked ? 1 : 0);
        }
    }

    public void ToggleAllUnlocked()
    {
        AllUnlocked = !AllUnlocked;

        _allUnlockLabel.text = SceneLocalizator.GetLocalizedString("Intro", 6, AllUnlocked ? 1 : 0);
    }
}
