using UnityEngine;

public class AsqContagioning : Module
{
    [Header("Minus = cleaning")]
    [SerializeField][Range(-10, 10)] private int _contagionVolume;
    [SerializeField] private bool _cleanAll;
    [SerializeField] private bool _setToCap;
    [SerializeField][Range(1, 11)] private int _capVolume = 1;

    public override void Asquiring()
    {
        if (_cleanAll)
        {
            ContagionHandler.Clear();
            return;
        }
        if (_setToCap && ContagionHandler.ContagionLevel < _capVolume)
        {
            ContagionHandler.AddContagion(_capVolume - ContagionHandler.ContagionLevel);
            return;
        }
        
        if (_contagionVolume < 0)
        {
            ContagionHandler.RemoveContagion(-_contagionVolume);
        } else if (_contagionVolume > 0)
        {
            ContagionHandler.AddContagion(_contagionVolume);
        }
    }
}
