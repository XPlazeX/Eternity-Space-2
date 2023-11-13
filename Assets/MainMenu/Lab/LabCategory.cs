using UnityEngine;

[CreateAssetMenu(fileName = "LabCategory", menuName = "Teleplane2/LabCategory", order = 0)]
public class LabCategory : ScriptableObject 
{
    [SerializeField] private Blueprint[] _blueprints;

    private int _currentID = 0;

    public Blueprint SelectBlueprint(int ID)
    {
        _currentID = ID;
        return _blueprints[_currentID];
    }

    public Blueprint NextBlueprint()
    {
        _currentID ++;
        if (_currentID >= _blueprints.Length)
            _currentID = 0;

        return _blueprints[_currentID];
    }

    public Blueprint PreviousBlueprint()
    {
        _currentID --;
        if (_currentID < 0)
            _currentID = _blueprints.Length - 1;

        return _blueprints[_currentID];
    }
}
