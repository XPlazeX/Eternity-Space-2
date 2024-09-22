using UnityEngine;

public class HideAbility : MonoBehaviour
{
    void Start()
    {
        SceneStatics.UICore.GetComponent<AbilityUI>().Hide();
    }

}
