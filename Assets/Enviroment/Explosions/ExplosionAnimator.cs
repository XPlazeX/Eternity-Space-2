using System.Collections;
using UnityEngine;

public class ExplosionAnimator : MonoBehaviour
{
    [Header("Set this component on any child of Explosion and bind with them")]
    [SerializeField] private bool _affectColor;
    [SerializeField] private Gradient _colorProgression;
    [SerializeField] private AnimationCurve _scaleProgression;
    [SerializeField] private float _scaleMultiplier = 1f;

    private float _animationTime;

    public void Initialize(float animationTime)
    {
        _animationTime = animationTime;
    }

    public void Play()
    {
        StartCoroutine(Execution());
    }

    private IEnumerator Execution()
    {
        float timer = _animationTime;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while (timer > 0f)
        {
            if (_affectColor)
                sr.color = _colorProgression.Evaluate(1f - (timer / _animationTime));

            transform.localScale = Vector3.one * _scaleProgression.Evaluate(1f - (timer / _animationTime)) * _scaleMultiplier;

            timer = Mathf.Clamp(timer - Time.deltaTime, 0f, _animationTime);

            yield return null;
        }
    }
}
