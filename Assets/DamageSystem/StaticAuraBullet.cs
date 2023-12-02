using System.Collections;
using UnityEngine;

public class StaticAuraBullet : StaticBullet
{
    [SerializeField] private float _damageDelay = 0.25f;

    private Collider2D _collider;

    private void Start() 
    {
        _collider = GetComponent<Collider2D>();
        StartCoroutine(Damaging());
    }

    private IEnumerator Damaging(){
        while (true)
        {
            _collider.enabled = true;

            yield return new WaitForSeconds(_damageDelay);

            _collider.enabled = false;
        }
    }
}
