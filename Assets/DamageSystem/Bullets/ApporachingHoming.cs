using UnityEngine;
using System.Collections;

public class ApporachingHoming : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private float _powerHoming;

    private Transform _trackingTransform;
    private IEnumerator _homingCoroutine;

    private bool _workable;
    private float _homingMultiplier = 1f;

    private void OnEnable() {
        _homingMultiplier = ShipStats.GetValue("HomingEfficiencyMultiplier");
        _workable = true;
    }

    private void OnDisable() {
        _workable = false;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (!_workable)
            return;
            
        if (other.CompareTag(_targetTag))
        {
            _homingCoroutine = Homing(other.transform);
            StartCoroutine(_homingCoroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!_workable)
            return;

        if (other.CompareTag(_targetTag) && _homingCoroutine != null)
        {
            StopCoroutine(_homingCoroutine);
        }
    }

    private IEnumerator Homing(Transform target)
    {
        while (target != null)
        {
            _rotatingTransform.up = SceneStatics.FlatVector(Vector3.RotateTowards(_rotatingTransform.up, target.position - _rotatingTransform.position, _powerHoming * Time.deltaTime * _homingMultiplier, 0f));

            yield return new WaitForFixedUpdate();
        }
    }

}
