using UnityEngine;

public class EnemyHealthBar : PullableObject
{
    private SpriteRenderer _spriteRenderer;
    private Transform _parent;
    private int _startHP;
    private float _offsetY;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void SetDefaultStats() 
    {}

    public void Initialize(int startHP)
    {
        _startHP = startHP;
        SetHP(startHP);

        _parent = transform.parent;

        if (_parent.GetComponent<_Invisible>() != null)
        {
            _parent.GetComponent<DamageBody>().DamageTaking -= SetHP;
            gameObject.SetActive(false);
            return;
        }

        if (_parent.GetComponent<BoxCollider2D>() != null)
            _offsetY = _parent.GetComponent<BoxCollider2D>().size.y;
        else
            _offsetY = 2f;
            
        transform.parent = null;
    }

    public void SetHP(int hitPoints)
    {
        _spriteRenderer.color = SceneStatics.HealthGradient.Evaluate((float)hitPoints / _startHP);

        float countBlocks = Mathf.Ceil(hitPoints / 10f);
        float rows = Mathf.Ceil(countBlocks / 20f);
        _spriteRenderer.size = new Vector2(Mathf.Ceil(countBlocks / rows), rows);
    }

    private void FixedUpdate() {
        if (_parent != null)
            transform.position = _parent.position + Vector3.up * _offsetY;
        
        else
            gameObject.SetActive(false);
    }
}
