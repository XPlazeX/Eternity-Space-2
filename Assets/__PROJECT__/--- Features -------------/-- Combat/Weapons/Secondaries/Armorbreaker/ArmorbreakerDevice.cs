using UnityEngine;

public class ArmorbreakerDevice : Device
{
    [SerializeField] private float cooldown = 15f;
    [SerializeField] private float focusTime = 3f;
    [SerializeField] private FocusLine[] focusLines;
    [SerializeField] private Gradient focusGradient;

    private float _cooldown;
    private float _focusTime;
    private bool _focusing;

    public override void Load()
    {
        base.Load();
        HideFocusLines();
    }

    public override float GetChargeNormalized()
    {
        return Mathf.Clamp01(1f - (_cooldown / cooldown));
    }

    public override float GetChargeRaw()
    {
        return _cooldown;
    }

    protected override void Update()
    {
        _cooldown -= ESTime.worldDeltaTime;

        base.Update();
        if (!MainWeaponHandler.CanUseWeapons) return;

        if (Active && PlayerInput.SecondaryFireDown && _cooldown <= 0f)
        {
            StartRelease();
        }
        else if (Active && PlayerInput.SecondaryFireUp)
        {
            _focusing = false;
            _focusTime = 0f;
            HideFocusLines();
        }
        else if (_focusing)
        {
            Releasing();
        }
    }

    public override void StartRelease()
    {
        _focusing = true;
        _focusTime = 0f;

        Vector3 focusPosition = _barrels[0].position;
        for (int i = 0; i < focusLines.Length; i++)
        {
            focusLines[i].line.transform.position = focusPosition;
            focusLines[i].line.transform.localRotation = Quaternion.Euler(0f, 0f, focusLines[i].angle);
            // focusLines[i].line.transform.SetLocalPositionAndRotation(
            //     focusPosition,
            //     Quaternion.Euler(0f, 0f, focusLines[i].angle));
        }

        base.StartRelease();
    }

    public override void Releasing()
    {
        _focusTime += ESTime.worldDeltaTime;
        float focusNormalized = Mathf.Clamp01(_focusTime / focusTime);
        Vector3 focusPosition = _barrels[0].position;
        Color focusColor = focusGradient.Evaluate(focusNormalized);

        for (int i = 0; i < focusLines.Length; i++)
        {
            LineRenderer line = focusLines[i].line;
            line.transform.position = focusPosition;
            line.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(focusLines[i].angle, 0f, focusNormalized));
            // line.transform.SetLocalPositionAndRotation(
            //     focusPosition,
            //     Quaternion.Euler(0f, 0f, Mathf.Lerp(focusLines[i].angle, 0f, focusNormalized)));
            SetFirstColorKey(line, focusColor);
        }

        if (_focusTime >= focusTime)
        {
            Push();
        }
        base.Releasing();
    }

    private void Push()
    {
        _cooldown = cooldown;

        SpawnBullet(_barrels[0].position, Player.Orientation.eulerAngles.z);
        MuzzleFlash(_barrels[0].position);

        PlaySound();

        _focusing = false;
        _focusTime = 0f;
        HideFocusLines();
    }

    private void HideFocusLines()
    {
        for (int i = 0; i < focusLines.Length; i++)
        {
            SetFirstColorKey(focusLines[i].line, Color.black);
        }
    }

    private static void SetFirstColorKey(LineRenderer line, Color color)
    {
        Gradient gradient = line.colorGradient;
        GradientColorKey[] colorKeys = gradient.colorKeys;
        colorKeys[0].color = color;
        gradient.SetKeys(colorKeys, gradient.alphaKeys);
        line.colorGradient = gradient;
    }

    [System.Serializable]
    private struct FocusLine
    {
        public LineRenderer line;
        public float angle;
    }
}
