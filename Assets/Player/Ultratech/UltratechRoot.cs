using UnityEngine;
using System.Collections;

public class UltratechRoot : MonoBehaviour
{
    public delegate void ultratechAction();

    public static event ultratechAction UltratechCharged;
    public static event ultratechAction UltratechEmpty;

    [SerializeField] private UltratechUI _UTUI;
    [SerializeField] private Explosion _explosion;

    public static bool NowUltrateched {get; set;} = false;

    private PullForObjects _UTexplosions;
    private Ultratech _ultratech;
    private GameObject _transformedShip;
    private int _ramNeeded = 3;
    private int _currentRams = 0;
    private float _UTDuration;
    private bool _active = true;

    private void Awake() 
    {
        NowUltrateched = false;
        
        _UTexplosions = new PullForObjects(_explosion as PullableObject);
        PlayerRamsHandler.RamSuccess += RamSuccess;
        PlayerShipData.PlayerDeath += Deactivate;
    }

    private void OnDisable() {
        PlayerRamsHandler.RamSuccess -= RamSuccess;
        PlayerShipData.PlayerDeath -= Deactivate;
    }

    public void Initialize(Ultratech ultratech)
    {
        _ultratech = ultratech;

        _transformedShip = ultratech.TransformedShip;
        _ramNeeded = (ultratech.RamsReload + ShipStats.GetIntValue("AdditiveRamsToUltratech"));
        _UTDuration = ultratech.TransformationDuration;

        _currentRams = 0;
        _UTUI.SetUTIcon(ultratech.Icon);
        _UTUI.FillImage(0);
        _UTUI.UpdateTextInfo(_ramNeeded);
        print("Ultratech initialized");
    }

    private void RamSuccess()
    {
        print("UT Ram active");
        if (!_active)
            return;
            
        print("UT Ram success");
        if (_currentRams < _ramNeeded)
            _currentRams ++;

        if (_currentRams == _ramNeeded)
        {
            _UTUI.ToggleInteractability(true);
            UltratechCharged?.Invoke();
        }

        _UTUI.FillImage((float)_currentRams / _ramNeeded);
        _UTUI.UpdateTextInfo(_ramNeeded - _currentRams);
        
    }

    public void Transformation()
    {
        if (!_active)
            return;

        _UTUI.ToggleInteractability(false);
        _UTUI.UpdateTextInfo(-1);

        _currentRams = 0;

        // Action
        Explode();
        Player.ReplacePlayer(_transformedShip);
        _ultratech.OnUse();

        StartCoroutine(UltratechLife(_UTDuration * ShipStats.GetValue("UltratechDurationMultiplier")));
    }

    private IEnumerator UltratechLife(float lifetime)
    {
        NowUltrateched = true;

        float timer = lifetime;

        while (timer > 0)
        {
            _UTUI.FillImage(timer / lifetime);

            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        Explode();
        Player.ReturnFirstPlayer();
        _UTUI.UpdateTextInfo(_ramNeeded);

        NowUltrateched = false;
        UltratechEmpty?.Invoke();
    }

    public void StopTransformationImmediately()
    {
        if (!NowUltrateched)
            return;
        
        else
        {
            StopAllCoroutines();
            Explode();
            Player.ReturnFirstPlayer();
            _UTUI.FillImage(0);
            _UTUI.UpdateTextInfo(_ramNeeded);
            NowUltrateched = false;
        }
    }

    public void Deactivate()
    {
        print("Deactivate UT Root");
        _active = false;
        PlayerRamsHandler.RamSuccess -= RamSuccess;
        StopTransformationImmediately();
        _UTUI.Disable();
    }

    private void Explode()
    {
        Transform explosion = _UTexplosions.GetGameObject().GetComponent<Transform>();

        explosion.position = Player.PlayerTransform.position;
    }
}
