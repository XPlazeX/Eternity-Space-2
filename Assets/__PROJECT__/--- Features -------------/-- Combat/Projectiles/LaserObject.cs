using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageSystem;

public class LaserObject : AttackObject
{
    public event System.Action<GameObject> Collided;
    public event System.Action<IDamagable> Hitted;

    [SerializeField] private float _warningTime = 0.7f;
    [SerializeField] private float _damageTick = 0.1f;
    [SerializeField] private float startOffset = 0.3f;

    [Header("Laser behaviour")]
    [SerializeField] private int _maxPiercedTargets = 1;
    [SerializeField] private bool _impulse = false;

    [Header("Raycast")]
    [SerializeField] private int _raycastBufferSize = 16;
    [SerializeField] private float _laserRadius = 0.2f;
    [Space()]
    [SerializeField] private _ExplosionBullet explosionComponent;
    [Header("Visual")]
    [SerializeField] private float fadeTime = 0.3f;

    private LineRenderer _lineRenderer;
    private Color _startColor;
    private Gradient _fadingGradient = new Gradient();

    private RaycastHit2D[] _hitsBuffer;
    private readonly HashSet<IDamagable> _processedDamagables = new HashSet<IDamagable>();
    private Coroutine _laserCoroutine;
    private Coroutine _fadingCoroutine;
    private bool _laserEnabled;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _startColor = _lineRenderer.colorGradient.colorKeys[0].color;
        _fadingGradient = MakeGradient(_startColor, Color.clear, 1f, 0f);

        _hitsBuffer = new RaycastHit2D[Mathf.Max(4, _raycastBufferSize)];
    }

    public void CreateLaser(Transform origin, float maxDistance, LayerMask mask, float lifetime)
    {
        StartLaser(origin, maxDistance, mask, Mathf.Max(lifetime, 0.001f));
    }

    public void LaserOn(Transform origin, float maxDistance, LayerMask mask)
    {
        StartLaser(origin, maxDistance, mask, null);
    }

    public void LaserOff()
    {
        if (!_laserEnabled && _laserCoroutine == null)
            return;

        _laserEnabled = false;

        if (_laserCoroutine != null)
        {
            StopCoroutine(_laserCoroutine);
            _laserCoroutine = null;
        }

        StartFading();
    }

    private void StartLaser(Transform origin, float maxDistance, LayerMask mask, float? lifetime)
    {
        StopActiveCoroutines();

        _processedDamagables.Clear();
        _laserEnabled = true;
        _laserCoroutine = StartCoroutine(Laser(origin, maxDistance, mask, lifetime));
    }

    private IEnumerator Laser(Transform origin, float maxDistance, LayerMask mask, float? lifetime)
    {
        float time = _warningTime;
        float timer = 0f;
        _lineRenderer.colorGradient = GetFadingMonoGradient(0.9f);

        while (_laserEnabled && timer < time)
        {
            if (origin == null)
            {
                StopLaserImmediately();
                yield break;
            }

            _lineRenderer.SetPositions(new Vector3[2]
            {
                origin.position + origin.up * startOffset,
                origin.position + origin.up * maxDistance
            });

            timer += ESTime.worldDeltaTime;
            yield return null;
        }

        timer = 0f;
        _lineRenderer.colorGradient = GetFadingMonoGradient(0f);

        bool impulseDone = false;
        float timerHurt = 0f;

        while (_laserEnabled && (!lifetime.HasValue || timer < lifetime.Value))
        {
            if (origin == null)
            {
                StopLaserImmediately();
                yield break;
            }

            Vector2 rayOrigin = SceneStatics.FlatVector(origin.position + origin.up * startOffset);
            Vector2 rayDirection = SceneStatics.FlatVector(origin.up);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(
            rayOrigin,
            _laserRadius,
            rayDirection,
            maxDistance,
            mask
        );

            float beamLength = maxDistance;

            if (hits.Length > 1)
            {
                System.Array.Sort(hits, 0, hits.Length, RaycastHit2DDistanceComparer.Instance);
            }

            if (_impulse)
            {
                if (!impulseDone)
                {
                    ProcessHits(hits, hits.Length, maxDistance, out beamLength);
                    impulseDone = true;
                }
                else
                {
                    EvaluateBeamLengthOnly(hits, hits.Length, maxDistance, out beamLength);
                }
            }
            else
            {
                if (timerHurt <= 0f)
                {
                    ProcessHits(hits, hits.Length, maxDistance, out beamLength);
                    timerHurt = _damageTick;
                }
                else
                {
                    EvaluateBeamLengthOnly(hits, hits.Length, maxDistance, out beamLength);
                }
            }

            Vector3 endPos = origin.position + origin.up * beamLength;
            _lineRenderer.SetPositions(new Vector3[2] { origin.position + origin.up * startOffset, endPos });

            timer += ESTime.worldDeltaTime;
            timerHurt -= ESTime.worldDeltaTime;

            yield return null;
        }

        _laserEnabled = false;
        _laserCoroutine = null;
        StartFading();
    }

    private void ProcessHits(RaycastHit2D[] hits, int hitCount, float maxDistance, out float beamLength)
    {
        beamLength = maxDistance;
        int piercedTargets = 0;
        _processedDamagables.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];

            if (hit.collider == null)
                continue;

            Collided?.Invoke(hit.collider.gameObject);

            if (explosionComponent != null)
            {
                explosionComponent.SpawnExplosion(hit.point);
            }

            IDamagable damagable = hit.collider.GetComponentInParent<IDamagable>();

            // Не цель — твердая преграда.
            if (damagable == null)
            {
                beamLength = hit.distance;
                return;
            }

            // Если у цели несколько коллайдеров — обрабатываем ее один раз.
            if (!_processedDamagables.Add(damagable))
                continue;

            // Сначала всегда наносим урон этой цели.
            bool inflicted = InflictDamage(damagable, out bool killed);

            if (inflicted)
            {
                Hitted?.Invoke(damagable);
            }
            // damageBody.TakeDamage(Damage);

            // Если пробитий больше не осталось — останавливаемся на этой цели.
            if (piercedTargets >= _maxPiercedTargets)
            {
                beamLength = hit.distance;
                return;
            }

            // Иначе считаем, что эту цель мы пробили и летим дальше.
            piercedTargets++;
        }

        beamLength = maxDistance;
    }

    private void EvaluateBeamLengthOnly(RaycastHit2D[] hits, int hitCount, float maxDistance, out float beamLength)
    {
        beamLength = maxDistance;
        int piercedTargets = 0;
        _processedDamagables.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];

            if (hit.collider == null)
                continue;

            Collided?.Invoke(hit.collider.gameObject);

            IDamagable damagable = hit.collider.GetComponentInParent<IDamagable>();

            if (damagable == null)
            {
                beamLength = hit.distance;
                return;
            }

            if (!_processedDamagables.Add(damagable))
                continue;

            // Визуально логика должна совпадать с боевой:
            // эта цель входит в допустимую цепочку попаданий,
            // но если пробивать ее уже нельзя — луч заканчивается на ней.
            if (piercedTargets >= _maxPiercedTargets)
            {
                beamLength = hit.distance;
                return;
            }

            piercedTargets++;
        }
    }

    private IEnumerator Fading()
    {
        float time = fadeTime;
        float timer = 0f;

        while (timer < time && time > 0f)
        {
            _lineRenderer.colorGradient = GetFadingMonoGradient(timer / time);

            timer += ESTime.worldDeltaTime;
            yield return null;
        }

        _fadingCoroutine = null;
        gameObject.SetActive(false);
    }

    private void StartFading()
    {
        if (_fadingCoroutine != null)
            StopCoroutine(_fadingCoroutine);

        _fadingCoroutine = StartCoroutine(Fading());
    }

    private void StopActiveCoroutines()
    {
        if (_laserCoroutine != null)
        {
            StopCoroutine(_laserCoroutine);
            _laserCoroutine = null;
        }

        if (_fadingCoroutine != null)
        {
            StopCoroutine(_fadingCoroutine);
            _fadingCoroutine = null;
        }
    }

    private void StopLaserImmediately()
    {
        _laserEnabled = false;
        _laserCoroutine = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _laserEnabled = false;
        _laserCoroutine = null;
        _fadingCoroutine = null;
    }

    private Gradient GetFadingMonoGradient(float time = 0f)
    {
        Color fadingColor = _fadingGradient.Evaluate(time);
        float fadingAlpha = fadingColor.a;

        return MakeGradient(fadingColor, fadingColor, fadingAlpha, fadingAlpha);
    }

    private Gradient MakeGradient(Color startColor, Color endColor, float startAlpha, float endAlpha)
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = startColor;
        colorKey[0].time = 0f;
        colorKey[1].color = endColor;
        colorKey[1].time = 1f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = startAlpha;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = endAlpha;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }

    private sealed class RaycastHit2DDistanceComparer : IComparer<RaycastHit2D>
    {
        public static readonly RaycastHit2DDistanceComparer Instance = new RaycastHit2DDistanceComparer();

        public int Compare(RaycastHit2D a, RaycastHit2D b)
        {
            return a.distance.CompareTo(b.distance);
        }
    }
}
