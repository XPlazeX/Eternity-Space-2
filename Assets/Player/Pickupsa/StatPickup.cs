using UnityEngine;

public class StatPickup : Pickup
{
    [SerializeField] private float _effectDuration;
    [SerializeField] private ShipStatModifier[] _modifiers;
    [SerializeField] private bool _playPowerUp;

    protected override void Picked()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Enforce();
        }
        
        GameObject.FindWithTag("BetweenScenes").GetComponent<TimedDelegator>().FuseAction(NegativeEffect, _effectDuration);

        if (_playPowerUp)
            SceneStatics.UICore.GetComponent<PlayerUI>().PlayEffect(PlayerUI.Effect.PowerUp, _effectDuration);

        base.Picked();
    }

    private void NegativeEffect()
    {
        for (int i = 0; i < _modifiers.Length; i++)
        {
            _modifiers[i].Negative();
        }
    }
}
