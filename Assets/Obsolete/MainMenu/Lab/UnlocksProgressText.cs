using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UnlocksProgressText : MonoBehaviour
{
    [SerializeField] private UnlockRequire[] _requiringUnlocks;

    private Text _label;

    private void OnEnable() {
        if (_label == null)
            _label = GetComponent<Text>();

        int counter = 0;

        for (int i = 0; i < _requiringUnlocks.Length; i++)
        {
            if (Unlocks.HasUnlock(_requiringUnlocks[i]))
                counter++;
        }

        _label.text = $"{counter}/{_requiringUnlocks.Length}";
    }
}
