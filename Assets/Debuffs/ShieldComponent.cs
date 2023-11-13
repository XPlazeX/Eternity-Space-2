using System.Collections;
using UnityEngine;

public class ShieldComponent : PullableObject
{
    [SerializeField] private Animator _animator;

    private SpriteRenderer _mySR;
    private Transform _carrierTransform;
    private int _shieldMax;
    private int _currentShield;

    bool _recoverable = false;
    bool _breaking = false;
    float _waitRecover = -1f;

    private void Start() {
        _mySR = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() {
        if (_breaking == true)
            return;

        if (!_breaking && _carrierTransform == null)
            BreakShield();

        transform.position = _carrierTransform.position;
        transform.rotation = _carrierTransform.rotation;

        if (_recoverable)
        {
            _waitRecover -= Time.deltaTime;
            if ((_currentShield < _shieldMax) && _waitRecover <= 0)
            {
                UpdateSP(_shieldMax);
                _carrierTransform.GetComponent<DamageBody>().GetShield(_shieldMax);
            }
        }
    }

    protected override void SetDefaultStats() 
    {
        _breaking = false;
        if (_mySR != null)
            _mySR.color = new Color(_mySR.color.r, _mySR.color.g, _mySR.color.b, 1f);

    }

    private void StartRecovering()
    {
        _waitRecover = 5f;
    }

    public void InitializeCarrier(Transform carrier, int maxSP)
    {
        _recoverable = false;
        _carrierTransform = carrier;
        _shieldMax = maxSP;
        _currentShield = _shieldMax;

        GetComponent<SpriteRenderer>().sprite = carrier.GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().flipY = carrier.GetComponent<SpriteRenderer>().flipY;

        transform.position = _carrierTransform.position;
        transform.rotation = _carrierTransform.rotation;
        transform.localScale = Vector3.one * 1.1f;

        if (carrier.GetComponent<DamageBody>().GetType() == typeof(DamageBody))
            _recoverable = true;
    }

    public void UpdateSP(int newSP)
    {
        _animator.SetTrigger("TakeDamage");
        _mySR.color = new Color(_mySR.color.r, _mySR.color.g, _mySR.color.b, ((float)newSP / _shieldMax) + 0.3f);

        if (newSP < _shieldMax)
            StartRecovering();

        _currentShield = newSP;
        print($"shield update : {newSP}");
    }

    public void BreakShield()
    {
        if (_breaking == true)
            return;
        //_animator.SetTrigger("Break");
        StartCoroutine(Breaking());
        _carrierTransform = null;
        _breaking = true;
    }

    //public void StartBreaking() => StartCoroutine(Breaking());

    public void Death() => gameObject.SetActive(false);

    private IEnumerator Breaking()
    {
        float timer = 0.5f;
        while (timer > 0)
        {
            _mySR.color = new Color(_mySR.color.r, _mySR.color.g, _mySR.color.b, 0.3f * (timer / 0.5f));
            transform.localScale = Vector3.one * (1.1f + (2f * (0.5f - timer)));

            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
