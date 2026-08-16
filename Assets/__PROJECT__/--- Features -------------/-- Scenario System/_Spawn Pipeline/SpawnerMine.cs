using UnityEngine;

public class SpawnerMine : MonoBehaviour
{
    [SerializeField] private DamageBody spawningDamageBody;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool animateScale = false;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    private float _animationTimer;
    private Transform _animatingTransform;

    public void InitSpawn()
    {
        Spawn();
    }

    private void Spawn()
    {
        DamageBody db = EnemySpawner.Spawn(spawningDamageBody, spawnPoint.position);
        if (animateScale)
        {
            _animatingTransform = db.transform;
            _animatingTransform.localScale = Vector3.one * startScale;
            _animationTimer = 0f;
        }
    }

    void Update()
    {
        if (_animationTimer > animationTime || _animatingTransform == null) return;

        _animationTimer += ESTime.worldDeltaTime;

        if (_animationTimer >= animationTime)
        {
            _animatingTransform.localScale = Vector3.one;
            _animatingTransform = null;
            return;
        }

        _animatingTransform.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, animationCurve.Evaluate(_animationTimer / animationTime));
    }
}
