using UnityEngine;

public class ParticleShleif : PullableObject
{
    private ParticleSystem _particleSystem;
    private TrailRenderer _trail;

    private void Start() {
        _particleSystem = GetComponent<ParticleSystem>();
        _trail = GetComponent<TrailRenderer>();

        if (_particleSystem == null && _trail == null)
            return;

        var pSMain = _particleSystem.main;
        pSMain.startRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
        
        var rendererPS = _particleSystem.GetComponent<ParticleSystemRenderer>();
        rendererPS.material.mainTexture = GetComponent<SpriteRenderer>().sprite.texture;
    }

    protected override void SetDefaultStats()
    {
        if (_particleSystem != null)
        {
            var pSMain = _particleSystem.main;
            pSMain.startRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
        }
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        if (_particleSystem != null)
            _particleSystem.Clear();
        if (_trail != null)
            _trail.Clear();  
    }
}
