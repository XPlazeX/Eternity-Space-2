using UnityEngine;

public class ToggleByContagion : MonoBehaviour
{
    private enum CompareType
    {
        LessInqlusive = 0,
        Equals = 1,
        MoreInqlusive = 2
    }
    [SerializeField] private GameObject _togglingObject;
    [SerializeField] private CompareType _compareType;
    [SerializeField] private int _checkingValue;

    private void OnEnable() {
        ContagionHandler.ContagionChanged += Check;
        Check();
    }

    private void OnDisable() {
        ContagionHandler.ContagionChanged -= Check;
    }

    public void Check()
    {
        switch (_compareType)
        {
            case CompareType.LessInqlusive:
                _togglingObject.SetActive(ContagionHandler.ContagionLevel <= _checkingValue);
                break;
            case CompareType.Equals:
                _togglingObject.SetActive(ContagionHandler.ContagionLevel == _checkingValue);
                break;
            case CompareType.MoreInqlusive:
                _togglingObject.SetActive(ContagionHandler.ContagionLevel >= _checkingValue);
                break;
            default:
                break;
        }
    }
}
