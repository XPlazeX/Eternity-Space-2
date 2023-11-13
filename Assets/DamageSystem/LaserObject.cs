using System.Collections;
using UnityEngine;
using DamageSystem;

public class LaserObject : AttackObject
{
    [SerializeField] private float _warningTime = 0.7f;
    [SerializeField] private float _damageTick = 0.1f;

    private LineRenderer _lineRenderer;
    private Color _startColor;
    private Gradient _fadingGradient = new Gradient();

    private void Awake() 
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _startColor = _lineRenderer.colorGradient.colorKeys[0].color;

        _fadingGradient = MakeGradient(_startColor, Color.clear, 1f, 0f);
    }

    public void CreateLaser(Transform origin, float maxDistance, LayerMask mask, float lifetime)
    {
        StartCoroutine(Laser(origin, maxDistance, mask, lifetime));
    }

    private IEnumerator Laser(Transform origin, float maxDistance, LayerMask mask, float lifetime)
    {
        float time = _warningTime;
        float timer = 0f;
        _lineRenderer.colorGradient = GetFadingMonoGradient(0.9f);

        while (timer < time)
        {
            if (origin == null)
                break;

            _lineRenderer.SetPositions(new Vector3[2] {origin.position, origin.position + origin.up * maxDistance});

            timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        time = lifetime;
        timer = 0f;
        _lineRenderer.colorGradient = GetFadingMonoGradient(0);

        float timerHurt = _damageTick;

        while (timer < time)
        {
            if (origin == null)
                break;

            RaycastHit2D hit = Physics2D.Raycast(SceneStatics.FlatVector(origin.position), SceneStatics.FlatVector(origin.up), maxDistance, mask);

            Vector3 pos = new Vector3();

            if (hit.collider == null)
            {
                pos = origin.position + origin.up * maxDistance;
            }
            else 
            {
                pos = hit.point;
                if (timerHurt <= 0)
                {
                    hit.collider.GetComponent<DamageBody>().TakeDamage(Damage);
                    if (Dev.IsLogging) print($"laser dmg : {Damage}");
                    timerHurt = _damageTick;
                }
            }

            _lineRenderer.SetPositions(new Vector3[2] {origin.position, pos});

            timer += Time.deltaTime;
            timerHurt -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(Fading());
    }

    private IEnumerator Fading()
    {
        float time = 0.3f;
        float timer = 0f;

        while (timer < time)
        {
            _lineRenderer.colorGradient = GetFadingMonoGradient(timer / time);

            timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }

    private Gradient GetFadingMonoGradient(float time = 0f)
    {
        Gradient gradient = new Gradient();

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
}
