using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private Button[] _navigationButtons;

    public void ToggleNavigation(bool tog)
    {
        for (int i = 0; i < _navigationButtons.Length; i++)
        {
            _navigationButtons[i].interactable = tog;
        }
    }
}
