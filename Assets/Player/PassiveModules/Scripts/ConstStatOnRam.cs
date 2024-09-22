using System.Collections.Generic;
using UnityEngine;
using StatsManipulating;

public class ConstStatOnRam : Module
{
    [SerializeField] private StatOperator[] _statOperators;
    [SerializeField] private int _armorUpgrade;
    [Space()]
    [SerializeField] private string _saveCollectionName;
    [SerializeField] private bool _consumeHP;
    [SerializeField] private int _consumingHP;

    private PlayerUI _playerUI;
    private int _scalesByLevel = 0;

    public override void Load()
    {
        _playerUI = SceneStatics.UICore.GetComponent<PlayerUI>();

        if (!GameSessionInfoHandler.ExistDataCollection(_saveCollectionName))
        {
            GameSessionInfoHandler.AddDataCollection(_saveCollectionName, new List<int>());
            GameSessionInfoHandler.AddValueToCollection(_saveCollectionName, 0);
        }

        int lastUpgrades = GameSessionInfoHandler.GetDataCollection(_saveCollectionName)[0];
        EnforceOperators(lastUpgrades);
        PlayerShipData.UpgradeArmor(_armorUpgrade * lastUpgrades, _armorUpgrade * lastUpgrades);

        PlayerRamsHandler.RamSuccess += OnRamSuccess;
        VictoryHandler.LevelVictored += OnLevelVictored;

        print($"CONST LOAD: {GameSessionInfoHandler.GetDataCollection(_saveCollectionName)[0]}");
    }

    private void OnDisable() {
        PlayerRamsHandler.RamSuccess -= OnRamSuccess;
        VictoryHandler.LevelVictored -= OnLevelVictored;
    }

    private void OnRamSuccess()
    {
        if (_consumeHP)
        {
            PlayerShipData.RegenerateHP(-_consumingHP);
        }

        _scalesByLevel ++;
        if (_armorUpgrade > 0)
            PlayerShipData.RegenerateArmor(_armorUpgrade);

        EnforceOperators(1);

        _playerUI.PlayEffect(PlayerUI.Effect.PowerUp, 1.5f);
    }

    private void EnforceOperators(int count)
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < _statOperators.Length; j++)
            {
                _statOperators[j].Enforce();
            }
        }
    }

    private void OnLevelVictored()
    {
        GameSessionInfoHandler.ReplaceValueInCollection(_saveCollectionName, 0, GameSessionInfoHandler.GetDataCollection(_saveCollectionName)[0] + _scalesByLevel);
        print($"CONST UPGRADE: {GameSessionInfoHandler.GetDataCollection(_saveCollectionName)[0]}");
    }
}
