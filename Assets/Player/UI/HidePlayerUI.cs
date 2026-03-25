using UnityEngine;
using System.Collections;

public class HidePlayerUI : MonoBehaviour
{
    void Start()
    {
        SceneStatics.UICore.GetComponent<PlayerUI>().TogglePassiveUI(false);

        StartCoroutine(HidingShipUI());
    }

    private IEnumerator HidingShipUI()
    {
        while (!GameSessionLoader.CharacterLoaded)
        {
            yield return null;
        }

        GameObject weaponUI = GameObject.FindWithTag("WeaponCharge");
        CanvasGroup wcg = weaponUI.AddComponent<CanvasGroup>();
        wcg.alpha = 0;
    }
}
