using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnlockerButton : MonoBehaviour
{
    [SerializeField] UnlockRequire[] _requiredUnlocks;
    [SerializeField] private GameObject[] _hiddingGO;
    [SerializeField][Range(0, 4)] private int _beaconCode = 0;
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
            if (GlobalSaveHandler.GetSave().TotalMissionsTries % 5 != _beaconCode)
            {
                CanvasGroup cg = GetComponent<CanvasGroup>();
                cg.alpha = 0f;
                check = false;
                //cg.interactable = false;
                //GetComponent<Button>().interactable = check;
                //return;
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
