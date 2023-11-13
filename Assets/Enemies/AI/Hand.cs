using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private Transform[] _armParts;
    [SerializeField] private Transform _shoulder;
    [SerializeField] private float _armLength;
    [SerializeField] private float _shoulderOffset;
    [SerializeField] private float _shoulderSpeed;
    [SerializeField] private Transform _palm;
    [SerializeField] private Transform _palmUp;
    [SerializeField] private Transform _palmDown;
    [SerializeField] private bool _rightOrientation;
    [SerializeField] private bool _alwaysOrientation = false;

    private Transform _palmUpCash;
    private Transform _palmDownCash;
    private bool transformed;

    private void Awake() {
        if (_body == null)
            _body = transform;
        StartCoroutine(HandBending());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private IEnumerator HandBending()
    {
        float _halfArm = _armLength + (2 * _shoulderOffset);

        float _distance = 0f;
        float _shoulderHeight = 0f;

        Vector3 _middlePoint = new Vector3();
        Vector3 _armDir;
        Vector3 _shoulderNormal = new Vector3();
        Vector3 _shoulderTarget = new Vector3();

        while (true)
        {
            _distance = (_body.position - _palm.position).magnitude;

            if ((_halfArm * _halfArm) - (_distance / 2f) < 0f)
                _shoulderHeight = 0f;
            else
                _shoulderHeight = Mathf.Sqrt((_halfArm * _halfArm) - (_distance / 2f));

            _middlePoint = Vector3.Lerp(_body.position, _palm.position, 0.5f);

            _armDir = _middlePoint - _body.position;

            if (_rightOrientation)
            {
                if (_alwaysOrientation || (((_palm.position.y < _body.position.y) && (_palm.position.x < _body.position.x)) || ((_palm.position.y > _body.position.y) && (_palm.position.x > _body.position.x))))
                {
                    _shoulderNormal = Rotate90Vector(_armDir, true).normalized;
                }
                else
                    _shoulderNormal = Rotate90Vector(_armDir, false).normalized;
            }
            else
            {
                if (_alwaysOrientation || (((_palm.position.y < _body.position.y) && (_palm.position.x < _body.position.x)) || ((_palm.position.y > _body.position.y) && (_palm.position.x > _body.position.x))))
                {
                    _shoulderNormal = Rotate90Vector(_armDir, false).normalized;
                }
                else
                    _shoulderNormal = Rotate90Vector(_armDir, true).normalized;
            }

            
            _shoulderTarget = (_shoulderNormal * _shoulderHeight) + _middlePoint;

            _shoulder.position += (_shoulderTarget - _shoulder.position) * _shoulderSpeed * Time.deltaTime;

            _armParts[0].position = Vector3.Lerp(_body.position, _shoulder.position, 0.5f);
            _armParts[1].position = Vector3.Lerp(_shoulder.position, _palm.position, 0.5f);

            _armParts[0].up = _shoulder.position - _body.position;
            _armParts[1].up = _palm.position - _shoulder.position;
            
            yield return new WaitForFixedUpdate();
        }

    }

    private Vector3 Rotate90Vector(Vector3 vec, bool clockwise = true)
    {
        float x = vec.x;
        float y = vec.y;

        if (clockwise)
            return new Vector3(y, -x, 0f);
        else
            return new Vector3(-y, x, 0f);
    }
}
