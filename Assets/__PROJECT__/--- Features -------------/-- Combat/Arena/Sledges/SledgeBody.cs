using UnityEngine;

public class SledgeBody : MonoBehaviour
{
    public bool EngineAvailable {get; private set;} = true;
    public bool LeftThrustersAvailable {get; private set;} = true;
    public bool RightThrustersAvailable {get; private set;} = true;
    public bool ForwardThrustersAvailable {get; private set;} = true;
    public bool BackingThrustersAvailable {get; private set;} = true;
    public bool CatcherAvailable {get; private set;} = true;
    public bool JumperAvailable {get; private set;} = true;
}
