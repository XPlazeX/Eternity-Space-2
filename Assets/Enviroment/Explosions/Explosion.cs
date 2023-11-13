//using System.Collections;
using UnityEngine;

public class Explosion : PullableObject
{
    [SerializeField] private ParticleSystem[] _usingParticleSystems;
    [SerializeField] private ExplosionAnimator[] _explosionAnimators;
    [SerializeField] private float _animationTime = 0.8f;
    [SerializeField] private float _explosionPower;
    [SerializeField] private SoundObject _sound;

    public float ExplosionPower => _explosionPower;

    private float _timer;
    private bool _sounded = false;

    public override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < _explosionAnimators.Length; i++)
        {
            _explosionAnimators[i].Initialize(_animationTime);
        }
    }

    public void Mute() => _sounded = true;

    protected override void SetDefaultStats()
    {
        for (int i = 0; i < _usingParticleSystems.Length; i++)
        {
            _usingParticleSystems[i].Play();
        }
        
        for (int i = 0; i < _explosionAnimators.Length; i++)
        {
            _explosionAnimators[i].Play();
        }

        if (ExplosionPower > 0)
            CameraController.Shake(ExplosionPower);
        
        _sounded = false;
        _timer = _animationTime;
        //StartCoroutine(Disabling());
    }

    private void FixedUpdate() {
        if (!_sounded)
        {
            SoundPlayer.PlaySound(_sound, transform.position);
            _sounded = true;
        }

        _timer -= Time.fixedDeltaTime;

        if (_timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }

    // private IEnumerator Disabling()
    // {
        
    //     yield return new WaitForSeconds(_animationTime);
    //     gameObject.SetActive(false);
    // }
}
