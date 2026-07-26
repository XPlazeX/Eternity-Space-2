using UnityEngine;
using UnityEngine.UI;

public class InteractableUnlock : MonoBehaviour
{
    [Header("По умолчанию, используется компонент Button")]
    [SerializeField] private bool _unsteadUseCanvasGroup = false;
    [SerializeField] private UnlockRequire _unlockRequire;
    [SerializeField] private float _disabledAlpha = 0.33f;

    private void OnEnable() 
    {
        if (_unsteadUseCanvasGroup)
        {
            GetComponent<CanvasGroup>().interactable = Unlocks.HasUnlock(_unlockRequire);
            GetComponent<CanvasGroup>().alpha = Unlocks.HasUnlock(_unlockRequire) ? 1f : _disabledAlpha;
        }
        else
        {
            GetComponent<Button>().interactable = Unlocks.HasUnlock(_unlockRequire);
            Color imageColor = GetComponent<Image>().color;
        }
    } 
}
