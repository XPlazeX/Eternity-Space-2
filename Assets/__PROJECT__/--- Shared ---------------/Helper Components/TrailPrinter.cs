using UnityEngine;

public class TrailPrinter : MonoBehaviour
{
    private GameObject _trailObject;
    private float _trailTime;
    private GameObject _copy;

    private void OnEnable() {
        if (_trailObject == null)
        {
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();

            if (tr != null)
            {
                _trailTime = tr.time;
                _trailObject = tr.gameObject;
                _trailObject.SetActive(false);
            }
        }

        if (_trailObject != null)
        {
            Print();
        }
    }

    private void LateUpdate() {
        if (_copy != null)
        {
            _copy.transform.position = transform.position;
        }
    }

    void OnDisable()
    {
        if (_copy != null)
        {
            SelfDestruct selfDestruct = _copy.AddComponent<SelfDestruct>();
            selfDestruct.SetTimer(_trailTime);
        }
    }

    private void Print()
    {
        _copy = Instantiate(_trailObject, transform.position, Quaternion.identity);
        _copy.SetActive(true);
    }
}
