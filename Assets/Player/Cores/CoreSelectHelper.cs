using UnityEngine;
using UnityEngine.UI;

public class CoreSelectHelper : MonoBehaviour
{
    [SerializeField] private Text _capacitylabel;
    [SerializeField] private Text _descriptionLabel;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _lockedColor;
    [Header("Статистика")]
    [SerializeField] private Text _missionStarts;
    [SerializeField] private Text _successMissions;
    [SerializeField] private Text _perfectMissions;
    [SerializeField] private Text _signals;
    [SerializeField] private Text _defeats;
    [SerializeField] private Text _rams;
    [SerializeField] private Text _energy;
    [SerializeField] private Text _sigaretes;
    [SerializeField] private Text _contagionTotal;
    [SerializeField] private Text _contagionMax;

    private void Start() {
        UpdateStatistics();
    }

    public void SetContainment(int coreID, bool unlocked)
    {
        _capacitylabel.text = unlocked ? SceneLocalizator.GetLocalizedString("Cores", coreID, 1) : "? " + SceneLocalizator.GetLocalizedString("Game", 12, 0);
        _descriptionLabel.text = unlocked ? SceneLocalizator.GetLocalizedString("Cores", coreID, 2) : SceneLocalizator.GetLocalizedString("Cores", coreID, 3);
        _descriptionLabel.color = unlocked ? Color.white : _lockedColor;
        _capacitylabel.color = unlocked ? _normalColor : _lockedColor;
    }

    private void UpdateStatistics()
    {
        print($"Kilowatts: {Unlocks.ValueOfUnlock(931, true)}");
        _missionStarts.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 0) + Unlocks.ValueOfUnlock(3, true).ToString();
        _successMissions.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 1) + Unlocks.ValueOfUnlock(8, true).ToString();
        _perfectMissions.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 2) + Unlocks.ValueOfUnlock(9, true).ToString();
        _signals.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 3) + Unlocks.ValueOfUnlock(7, true).ToString();
        _defeats.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 4) + Unlocks.ValueOfUnlock(4, true).ToString();
        _rams.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 5) + Unlocks.ValueOfUnlock(930, true).ToString();
        _energy.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 6) + SceneStatics.PreferFloatView(0.001f * Unlocks.ValueOfUnlock(931, true)) + SceneLocalizator.GetLocalizedString("Game", 12, 0);
        _sigaretes.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 7) + Unlocks.ValueOfUnlock(933, true).ToString();
        _contagionTotal.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 8) + Unlocks.ValueOfUnlock(935, true).ToString();
        _contagionMax.text = SceneLocalizator.GetLocalizedString("Lobby", 6, 9) + Unlocks.ValueOfUnlock(934, true).ToString();
    }
}
