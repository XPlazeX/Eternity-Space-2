using UnityEngine;

public class AuriteOnDamageTaken : Module
{
    [SerializeField][Range(1, 100)] private int _healthDivider;
    [SerializeField] private int _auritePerDivide;

    VictoryHandler _victoryHandler;

    public override void Load()
    {
        // PlayerShipData.TakeHealthDamage += OnDamageTaken;
        _victoryHandler = SceneStatics.CharacterCore.GetComponent<VictoryHandler>();
    }

    private void OnDisable() {
        // PlayerShipData.TakeHealthDamage -= OnDamageTaken;
    }

    public void OnDamageTaken(int dmg)
    {
        //print($"tkd dmg: {dmg} rew {dmg % _healthDivider}");
        int rewards = dmg / _healthDivider;

        _victoryHandler.AddAurite(_auritePerDivide * rewards);
    }
}
