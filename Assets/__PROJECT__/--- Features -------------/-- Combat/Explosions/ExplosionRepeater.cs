using UnityEngine;

public class ExplosionRepeater : MonoBehaviour
{
    [SerializeField] private ExplosionObject explosionObject;
    [SerializeField] private float repeatTime = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private int cycles = 3;

    private int _cycled;
    private float _timer = 0f;

    void OnEnable()
    {
        _cycled = 0;
        _timer = 0f;
    }

    void FixedUpdate()
    {
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
    }
}
