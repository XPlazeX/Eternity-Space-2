using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class ColorByBulletLifetime : MonoBehaviour
{
    [SerializeField] private BulletColoring[] bulletColorings;

    private Bullet _bullet;

    private void OnEnable() 
    {
        if (_bullet == null) 
            _bullet = GetComponent<Bullet>();
        Evaluate(0);
    }

    void FixedUpdate()
    {
        if (_bullet != null)
            Evaluate(1f - _bullet.LifetimeLeftNormalized);
    }

    private void Evaluate(float t)
    {
        t = Mathf.Clamp01(t);

        for (int i = 0; i < bulletColorings.Length; i++)
        {
            bulletColorings[i].spriteRenderer.color = bulletColorings[i].colorProgression.Evaluate(t);
        }
    }

    [System.Serializable]
    private struct BulletColoring
    {
        public SpriteRenderer spriteRenderer;
        public Gradient colorProgression;
    }
}
