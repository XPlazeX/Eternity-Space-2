using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnlockerButton : MonoBehaviour
{
    const int beacon_code_count = 5;

    [SerializeField] UnlockRequire[] _requiredUnlocks;
    [SerializeField] private GameObject[] _hiddingGO;
    [SerializeField][Range(0, beacon_code_count - 1)] private int _beaconCode = 0;
    [Space()]
    [SerializeField] private bool _modColor = false;
    [SerializeField] private UnlockRequire[] _unlocksRequires;
    [SerializeField] private Image _coloringRect;
    [SerializeField] private Text _coloringText;
    [SerializeField] private Color _unlockedColor;
    [SerializeField] private Color _lockedColor;

    private void OnEnable() {
        bool check = true;

        for (int i = 0; i < _requiredUnlocks.Length; i++)
        {
            if (!_requiredUnlocks[i].Completed)
            {
                check = false;
                break;
            }
        }

        if (_beaconCode > 0)
        {
            if (Unlocks.ValueOfUnlock(8) % beacon_code_count != _beaconCode) // берется кол-во пройденных миссий
            {
                CanvasGroup cg = GetComponent<CanvasGroup>();
                cg.alpha = 0f;
                check = false;
            }
        }
        

        GetComponent<Button>().interactable = check;
        for (int i = 0; i < _hiddingGO.Length; i++)
        {
            _hiddingGO[i].SetActive(check);
        }

        if (!_modColor || !Unlocks.HasUnlocks(_unlocksRequires))
            return;

        if (check)
        {
            _coloringRect.color = _unlockedColor;
            _coloringText.color = _unlockedColor;
        }
        else
        {
            _coloringRect.color = _lockedColor;
            _coloringText.color = _lockedColor;
        }
    }

}
