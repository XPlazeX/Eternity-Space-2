using UnityEngine;

public class HomingTrap : MonoBehaviour
{
    [SerializeField] private bool infiniteTime = false;
    [SerializeField] private float workTime = 8f;
    [SerializeField] private ParticleSystem visualParticles;

    private float _timer;

    public bool Active {get; private set;} = false;

    private void OnEnable() 
    {
        if (!infiniteTime)
        {
            _timer = workTime;
        }

        Active = true;
        if (visualParticles != null)
            visualParticles.Play();
    }

    void Update()
    {
        if (infiniteTime || !Active) return;

        _timer -= ESTime.worldDeltaTime;

        if (_timer <= 0f)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        Active = true;
        if (visualParticles != null)
            visualParticles.Stop();
    }
}
