using UnityEngine;

public class WeaponUpgrader : Module
{
    [SerializeField][Range(-5, 5)] private int _upgradeLevel;

    public override void Asquiring()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.WeaponLevel += _upgradeLevel;
        save.WeaponLevel = Mathf.Clamp(save.WeaponLevel, 0, 5);

        GameSessionInfoHandler.RewriteSessionSave(save);

        GameObject.FindObjectOfType<WeaponService>().CheckLevel();
    }
}
