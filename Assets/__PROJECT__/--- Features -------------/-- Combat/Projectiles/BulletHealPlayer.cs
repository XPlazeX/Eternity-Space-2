using UnityEngine;

public class BulletHealPlayer : MonoBehaviour
{
    [SerializeField] private int _healValue;
    [SerializeField] private Bullet _bullet;

    private void OnEnable() {
        _bullet.Hitted += OnBulletHitted;
    }

    private void OnDisable() {
        _bullet.Hitted -= OnBulletHitted;
    }

    public void OnBulletHitted()
    {
        PlayerShipData.RegenerateHP(_healValue);
    }
}
