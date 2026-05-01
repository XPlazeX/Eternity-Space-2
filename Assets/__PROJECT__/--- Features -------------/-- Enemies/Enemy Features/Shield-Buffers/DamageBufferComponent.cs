using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageBufferComponent : PullableObject
{
    [SerializeField] private Color idleColor;
    [SerializeField] private Gradient damageGradient;
    [SerializeField] private float damageAnimationTime;

    private SpriteRenderer _mySR;
    bool _breaking = false;
    private Transform _carrierTransform;
    float _timer;

    private void Start() {
        _mySR = GetComponent<SpriteRenderer>();
    }

    public void InitializeCarrier(Transform carrier)
    {
        _carrierTransform = carrier;

        GetComponent<SpriteRenderer>().sprite = carrier.GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().flipY = carrier.GetComponent<SpriteRenderer>().flipY;

        transform.position = _carrierTransform.position;
        transform.rotation = _carrierTransform.rotation;
        transform.localScale = Vector3.one * 1.1f;

    }

    private void LateUpdate() {
        if (_breaking == true)
            return;

        if (!_breaking && _carrierTransform == null)
            BreakShield();

        transform.position = _carrierTransform.position;
        transform.rotation = _carrierTransform.rotation;

        if (_timer >= 0f)
        {
            _mySR.color = damageGradient.Evaluate(1f - (_timer / damageAnimationTime));

            if (_timer <= 0.0001f)
            {
                _mySR.color = idleColor;
                _timer = 0f;
            }

            _timer -= Time.deltaTime;
        }

    }

    public void UpdateDB()
    {
        _timer = damageAnimationTime;
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
        float timer = damageAnimationTime;
        while (timer > 0)
        {
            if (_mySR == null)
                _mySR = GetComponent<SpriteRenderer>();

            _mySR.color = new Color(_mySR.color.r, _mySR.color.g, _mySR.color.b, 0.3f * (timer / 0.5f));
            transform.localScale = Vector3.one * (1.1f + (2f * (damageAnimationTime - timer)));

            timer -= Time.deltaTime;
            yield return null;
        }

        _breaking = false;
        Death();
    }
}
