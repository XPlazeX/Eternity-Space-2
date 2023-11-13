using System.Collections.Generic;
using UnityEngine;

public class ParryingHandler : MonoBehaviour
{
    public delegate void parryHandler();
    public static event parryHandler ParrySuccess;

    [SerializeField] private Transform _parryCircle;
    [SerializeField] private ParringObject[] _parringObjects;

    private static RamShield _ramShield;
    private static List<PullForObjects> ParringPools = new List<PullForObjects>();

    public static int ArmorRegen {get; private set;} = 1;
    public static bool Initialized {get; private set;} = false;

    public void Initialize()
    {
        ParringPools.Clear();

        for (int i = 0; i < _parringObjects.Length; i++)
        {
            ParringPools.Add(new PullForObjects(_parringObjects[i] as ParringObject));
        }

        // SceneTransition.SceneTransit += Clear;
        Initialized = true;
    }

    private void OnDisable() {
        Initialized = false;
    }

    public static void Parry()
    {
        ParrySuccess?.Invoke();

        PlayerShipData.RegenerateArmor(ArmorRegen);
    }

    public static void BuffedParry()
    {
        if (_ramShield != null)
        {
            _ramShield.EnableShield(true);
            SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(PlayerUI.Effect.Parry);
        }
    }

    public static void ConstParry()
    {
        _ramShield = Player.PlayerObject.GetComponentInChildren<RamShield>();

        if (_ramShield == null)
            return;
            
        _ramShield.EnableShield(true);
        SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(PlayerUI.Effect.Parry);
    }

    public void SetParryCircle(float scale)
    {
        Transform circle = Instantiate(_parryCircle, Player.PlayerTransform.position, Quaternion.identity);
        circle.localScale = Vector3.one * scale;
        circle.SetParent(Player.PlayerTransform);
        _ramShield = Player.PlayerObject.GetComponentInChildren<RamShield>();
    }

    public static ParringObject GetParringObject(int index)
    {
        return ParringPools[index].GetGameObject().GetComponent<ParringObject>();
    }

    public static ParringObject GetForChangeParringObject(int index)
    {
        ParringPools[index].SampleChanged();
        return ParringPools[index].Sample as ParringObject;
    }
}
