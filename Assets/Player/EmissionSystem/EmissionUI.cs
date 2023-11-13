using UnityEngine;
using UnityEngine.UI;

public class EmissionUI : MonoBehaviour
{
    [SerializeField] private Button _emissionButton;
    [SerializeField] private Image _fillEmission;
    [SerializeField] private Animator _animator;

    private bool _prepared = false;

    private void PrepareToUse(bool tog)
    {
        _emissionButton.interactable = tog;
        _animator.SetBool("Ready", tog);

        _prepared = tog;
    }

    public void FillEmission(float value)
    {
        _fillEmission.fillAmount = Mathf.Clamp01(value);

        if ((value >= 1f) && (!_prepared))
        {
            PrepareToUse(true);
            ShipStats.IncreaseStat("BlockArmor", 5);
        }

        else if ((value < 1f) && (_prepared))
        {
            PrepareToUse(false);
            ShipStats.IncreaseStat("BlockArmor", -5);
        }
    }

    public void Emit() => _animator.SetTrigger("Emit");

    public void Hide() => _emissionButton.gameObject.SetActive(false);
}
