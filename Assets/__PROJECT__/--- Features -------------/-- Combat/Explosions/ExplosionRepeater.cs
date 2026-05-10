using UnityEngine;

public class ExplosionRepeater : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private ExplosionObject explosionObject;
    [SerializeField] private float scale = 1f;
    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.wheat;
    [SerializeField] private float repeatTime = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private int cycles = 3;

    private int _cycled;
    private float _timer = 0f;
    private bool _started;

    void OnEnable()
    {
        _cycled = 0;
        _timer = 0f;
        _started = false;
    }

    void Start()
    {
        if (autoStart) _started = true;
    }

    void FixedUpdate()
    {
        if (!_started) return;

        _timer -= Time.fixedDeltaTime;

        if (_timer < 0f)
        {
            Explode();
            _cycled ++;
            _timer = repeatTime;

            if (!loop &&_cycled >= cycles)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void Explode()
    {
        ExplosionObject explosion = Pool.Spawn(explosionObject, transform.position, transform.rotation);
        explosion.SetScale(scale);
        if (overrideColor)
            explosion.SetColor(color);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _started = true;
    }
}
