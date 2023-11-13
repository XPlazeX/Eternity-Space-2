using UnityEngine;
using System.Collections;

public class AuraBullet : Bullet
{
    public const float damageDelay = 0.25f;

    private Collider2D _collider;

    private void Awake() {
        Pierces = 999999;
        _collider = GetComponent<Collider2D>();
    }

    private IEnumerator Damaging(){
        while (true)
        {
            _collider.enabled = true;

            yield return new WaitForSeconds(damageDelay);

            _collider.enabled = false;
        }
    }

    protected override void SetDefaultStats()
    {
        base.SetDefaultStats();
        StartCoroutine(Damaging());
    }
}
