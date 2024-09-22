using UnityEngine;
using DamageSystem;

public class ReflectorModifier : Module
{
    [SerializeField] private bool _customDamageKey;
    [SerializeField] private DamageKey _catchingKey;
    [SerializeField] private bool _disableMirroring;
    [SerializeField] private int _additiveReflects;
    [SerializeField] private bool _setNewObject;
    [SerializeField] private PullableObject _newObject;

    private bool _loaded = false;

    public override void Load()
    {
        if (_customDamageKey)
            BulletReflector.ChangePlayerCathingKey(_catchingKey);

        if (_disableMirroring)
            BulletReflector.TogglePlayerMirroring(false);

        BulletReflector.AddPlayerReflectsCount(_additiveReflects);

        if (_newObject)
        {
            Player.PlayerTransform.GetComponent<CharacterReflector>().ChangeTriggeringObject(_newObject);
        }

        _loaded = true;
    }

    private void OnDisable() {
        if (!_loaded)
            return;

        BulletReflector.ChangePlayerCathingKey(DamageKey.Player);
        BulletReflector.TogglePlayerMirroring(true);
        BulletReflector.AddPlayerReflectsCount(-_additiveReflects);
    }
}
