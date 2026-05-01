using UnityEngine;

public class EnemyHealthBar : PooledObject
{
    private SpriteRenderer _spriteRenderer;
    private Transform _parent;
    private int _startHP;
    private float _offsetY;

    private bool _elite = false;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int startHP, bool elite)
    {
        _startHP = startHP;
        _elite = elite;

        SetHP(startHP);

        _parent = transform.parent;

        if (_parent.GetComponent<_Invisible>() != null)
        {
            _parent.GetComponent<DamageBody>().DamageTaking -= SetHP;
            _parent.GetComponent<DamageBody>().HealthModified -= OnHealthModified;
            gameObject.SetActive(false);
            return;
        }

        if (_parent.GetComponent<BoxCollider2D>() != null)
            _offsetY = _parent.GetComponent<BoxCollider2D>().size.y;
        else
            _offsetY = 2f;
            
        transform.parent = null;
    }

    protected override void ResetState()
    {
        base.ResetState();
    }

    public void SetHP(int hitPoints)
    {
        _spriteRenderer.color = (_elite ? SceneStatics.EliteHealthGradient : SceneStatics.HealthGradient).Evaluate((float)hitPoints / _startHP);

        float countBlocks = Mathf.Ceil(hitPoints / 10f);
        float rows = Mathf.Ceil(countBlocks / 20f);
        _spriteRenderer.size = new Vector2(Mathf.Ceil(countBlocks / rows), rows);
    }

    public void OnHealthModified(int newMaxHealth)
    {
        _startHP = newMaxHealth;
    }

    private void Update() {
        if (_parent != null)
            transform.position = _parent.position + Vector3.up * _offsetY;
        
        else
            gameObject.SetActive(false);
    }
}
