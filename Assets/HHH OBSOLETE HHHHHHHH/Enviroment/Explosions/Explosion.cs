//using System.Collections;
using UnityEngine;

public class Explosion : PullableObject
{
    [SerializeField] private ParticleSystem[] _usingParticleSystems;
    [SerializeField] private ExplosionAnimator[] _explosionAnimators;
    [SerializeField] private float _animationTime = 0.8f;
    [SerializeField] private float _explosionPower;
    [SerializeField] private float _explosionDelay = 0f;
    [SerializeField] private bool _uiSound = false;
    [SerializeField] private SoundObject _sound;
    [SerializeField] private Collider2D[] _togglingColliders;
    [SerializeField] private float _colliderEnableDelay;

    public float ExplosionPower => _explosionPower;

    private float _timer;
    private float _delayTimer;
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
        if (_explosionDelay == 0)
            PlayExplosion();
        else
            _delayTimer = _explosionDelay;

        if (_togglingColliders.Length > 0)
            ToggleColliders(false);
        //StartCoroutine(Disabling());
    }

    private void PlayExplosion()
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
    }

    private void FixedUpdate() 
    {
        if (_delayTimer > 0)
        {
            _delayTimer -= ESTime.worldFixedDeltaTime;

            if (_delayTimer <= 0)
            {
                PlayExplosion();
            }

            return;
        }

        if (!_sounded)
        {
            if (_uiSound)
                SoundPlayer.PlayUISound(_sound);
            else
                SoundPlayer.PlaySound(_sound, transform.position);
            _sounded = true;
        }

        _timer -= ESTime.worldFixedDeltaTime;

        if (_togglingColliders.Length > 0 && _timer < _animationTime - _colliderEnableDelay)
        {
            ToggleColliders(true);
        }

        if (_timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void ToggleColliders(bool tog)
    {
        for (int i = 0; i < _togglingColliders.Length; i++)
        {
            _togglingColliders[i].enabled = tog;
        }
    }

    // private IEnumerator Disabling()
    // {
        
    //     yield return new WaitForSeconds(_animationTime);
    //     gameObject.SetActive(false);
    // }
}
