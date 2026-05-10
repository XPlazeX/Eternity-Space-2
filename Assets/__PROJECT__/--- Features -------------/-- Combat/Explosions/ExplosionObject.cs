using UnityEngine;

public class ExplosionObject : PooledObject
{
    public event System.Action Detonated;

    [SerializeField] private float waitTime;
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem[] particleSystems;
    [Header("Shockwaves")]
    [SerializeField] private ExplosionAnimatedLayer[] explosionAnimatedLayers;
    [Header("Mask")]
    [SerializeField] private ExplosionAnimatedMask explosionAnimatedMask;
    [Header("Audio")]
    [SerializeField] private SoundObject sound;

    private float _timer;
    private float _animationTime;
    private bool _sounded = false;

    public bool IsDetonated {get; private set;}

    protected override void ResetState()
    {
        base.ResetState();

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Clear();
        }
        for (int i = 0; i < explosionAnimatedLayers.Length; i++)
        {
            explosionAnimatedLayers[i].Reset();
        }

        if (explosionAnimatedMask != null) explosionAnimatedMask.Reset();

        _animationTime = CalculateAnimationTime();
        _timer = 0f;
        _sounded = false;
    }

    private void OnEnable() 
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }

        if (!_sounded)
        {
            SoundPlayer.PlaySound(sound, transform.position);
            _sounded = true;
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].transform.localScale = Vector3.one * scale;
        }
    }

    public void Mute() => _sounded = true;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < waitTime) return;

        if (!IsDetonated)
        {
            IsDetonated = true;
            Detonated?.Invoke();
        }

        float elapsedTimer = _timer - waitTime;

        for (int i = 0; i < explosionAnimatedLayers.Length; i++)
        {
            explosionAnimatedLayers[i].Evaluate(elapsedTimer);
        }

        if (explosionAnimatedMask != null) explosionAnimatedMask.Evaluate(elapsedTimer);

        if (_timer >= _animationTime)
        {
            Release();
        }
    }

    private float CalculateAnimationTime()
    {
        float t = waitTime;

        for (int i = 0; i < particleSystems.Length; i++)
        {
            float d = ParticleSystemLifetimeUtility.GetRootDuration(particleSystems[i]);
            if (d > t) t = d;
        }

        for (int i = 0; i < explosionAnimatedLayers.Length; i++)
        {
            float d = explosionAnimatedLayers[i].Duration;
            if (d > t) t = d;
        }

        if (explosionAnimatedMask != null && explosionAnimatedMask.Duration > t) t = explosionAnimatedMask.Duration;

        return t;
    }

    public void SetColor(Color c)
    {
        for (int i = 0; i < explosionAnimatedLayers.Length; i++)
        {
            explosionAnimatedLayers[i].SetColor(c);
        }
    }
}

[System.Serializable]
public class ExplosionAnimatedLayer
{
    [SerializeField] private float waitTime = 0f;
    [SerializeField] private GameObject animatingObject;
    [SerializeField] private bool mayChangeColor = true;
    [SerializeField] private Gradient colorProgression;
    [SerializeField] private AnimationCurve scaleProgression;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private bool hideAfterTimer = false;

    private Gradient _targetGradient;
    private SpriteRenderer _sr;
    private bool _activated;

    public AnimationCurve ScaleProgression => scaleProgression;
    public float ScaleMultiplier => scaleMultiplier;
    public float WaitTime => waitTime;
    public float AnimationTime => animationTime;
    public float Duration => WaitTime + AnimationTime;

    public void Reset()
    {
        animatingObject.SetActive(false);
        _targetGradient = colorProgression;
        _activated = false;
        _sr = animatingObject.GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        if (!mayChangeColor) return;

        _targetGradient = GradientUtility.RecolorGradientKeepBrightness(_targetGradient, color);
    }

    public void Evaluate(float t)
    {
        if (t < waitTime) return;
        if (hideAfterTimer && t > waitTime + animationTime)
        {
            animatingObject.transform.localScale = Vector3.zero;
            return;
        }

        if (!_activated)
        {
            animatingObject.SetActive(true);
            _activated = true;
        }

        float elapse = Mathf.Clamp01((t - waitTime) / animationTime);

        animatingObject.transform.localScale = (Vector3.one * scaleProgression.Evaluate(elapse)) * scaleMultiplier;

        if (_sr != null)
            _sr.color = _targetGradient.Evaluate(elapse);
    }
}

[System.Serializable]
public class ExplosionAnimatedMask
{
    [SerializeField] private float waitTime;
    [SerializeField] private Transform maskTransform;
    [SerializeField] private AnimationCurve scaleProgression;
    [SerializeField] private float animationTime;
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private bool hideAfterTimer = true;

    public float Duration => waitTime + animationTime;

    public void Reset()
    {
        if (maskTransform == null) return;

        maskTransform.localScale = Vector3.zero;
    }

    public void Evaluate(float t)
    {
        if (t < waitTime || maskTransform == null) return;
        if (hideAfterTimer && t > waitTime + animationTime)
        {
            maskTransform.localScale = Vector3.zero;
            return;
        }

        float elapse = Mathf.Clamp01((t - waitTime) / animationTime);

        maskTransform.localScale = Vector3.one * scaleProgression.Evaluate(elapse) * scaleMultiplier;
    }
}
