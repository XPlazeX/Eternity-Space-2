using UnityEngine;

public class FinwhalAlt : MonoBehaviour
{
    private const int achievement_id = 902;

    private void Start() {
        if (Unlocks.HasUnlock(achievement_id))
        {
            Destroy(this);
        }
    }

    private void OnEnable() {
        // PlayerShipData.ChangeArmor += OnArmorChanged;
    }

    private void OnDisable() {
        // PlayerShipData.ChangeArmor -= OnArmorChanged;
    }

    private void OnArmorChanged(int newArm)
    {
        if (newArm >= 40)
        {
            Unlocks.NewUnlock(achievement_id);
            Destroy(this);
        }
    }
}
