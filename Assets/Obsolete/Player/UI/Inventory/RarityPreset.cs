using UnityEngine;

public class RarityPreset : MonoBehaviour
{
    [SerializeField] private Color _mainColor;
    [SerializeField] private Color _gearFirstColor;
    [SerializeField] private Color _gearSecondColor;

    public Color MainColor => _mainColor;
    public Color GearFirstColor => _gearFirstColor;
    public Color GearSecondColor => _gearSecondColor;

}
