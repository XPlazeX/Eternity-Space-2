using UnityEngine;

[RequireComponent(typeof(BulletSplitObject))]
public class SplitBullet : Bullet
{
    private BulletSplitObject _bulletSplitObject;

    private void Start() {
        _bulletSplitObject = GetComponent<BulletSplitObject>();
    }

    protected override void Deactivate()
    {
        _bulletSplitObject.Split();
        base.Deactivate();
    }
}
