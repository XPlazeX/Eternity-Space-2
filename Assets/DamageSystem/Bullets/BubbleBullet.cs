using UnityEngine;
using System.Collections;

public class BubbleBullet : Bullet
{
    [SerializeField] private string _targetTag;
    [SerializeField] private float _ramDelay;
    [SerializeField] private float _speedRam;

    private IEnumerator Bubbling()
    {
        yield return new WaitForSeconds(_ramDelay);
        transform.up = (GameObject.FindWithTag(_targetTag).transform.position - transform.position);
        base._speed = _speedRam;
    }

    protected override void SetDefaultStats()
    {
        base.SetDefaultStats();
        StartCoroutine(Bubbling());
    }
}
