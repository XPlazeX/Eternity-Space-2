using UnityEngine;

public class SecondChance : Module
{
    [SerializeField] private string _reviveCode = "revive1";
    [Space()]
    [SerializeField] private bool _newShip;
    [SerializeField] private int _newShipID;
    [SerializeField] private bool _rewriteHealth;
    [SerializeField][Range(0, 1f)] private float _revivingHealthPercentage;
    [SerializeField] private bool _setNewWeapon;
    [SerializeField] private string _newWeapon;

    public override void Asquiring()
    {
        ReviveManager.RegisterRevive(_reviveCode);
    }

    // public override void Load()
    // {
    //     ReviveManager.RegisterRevive(_reviveCode);
    // }

    private void OnEnable() {
        ReviveManager.TryingRevive += OnTryingRevive;
    }

    private void OnDisable() {
        ReviveManager.TryingRevive -= OnTryingRevive;
    }

    public void OnTryingRevive(string code)
    {
        if (code != _reviveCode)
        {
            return;
        }

        print($"<color=lime>REVIVE WITH CODE: {_reviveCode}</color>");

        if (_newShip)
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            save.ShipModel = _newShipID;
            GameSessionInfoHandler.RewriteSessionSave(save);

            CharacterLoader cl = SceneStatics.CharacterCore.GetComponent<CharacterLoader>();
            cl.StartCoroutine(cl.WritingShipHPData(_newShipID, _rewriteHealth ? _revivingHealthPercentage : 1f));
        }
        else if (!_newShip && _rewriteHealth)
        {
            CharacterLoader cl = SceneStatics.CharacterCore.GetComponent<CharacterLoader>();
            cl.StartCoroutine(cl.WritingShipHPData(GameSessionInfoHandler.GetSessionSave().ShipModel, _rewriteHealth ? _revivingHealthPercentage : 1f));
        }

        if (_setNewWeapon)
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            save.WeaponModel = _newWeapon;
            GameSessionInfoHandler.RewriteSessionSave(save);
        }
    }
}
