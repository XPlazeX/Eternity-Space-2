using UnityEngine;

public class BulletViewModifier : Module
{
    private const string halo_name = "Halo_2";

    [SerializeField] private Sprite _newSprite;
    [SerializeField] private bool _addTrail;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private bool _modColor;
    [SerializeField] private Color _bulletColor;
    [SerializeField] private bool _modHaloColor;
    [SerializeField] private Color _haloColor;
    [SerializeField] private bool _modScale;
    [SerializeField] private float _scaleMultiplier;

    public override void Load()
    {
        AttackPattern[] attackPatterns = GameObject.FindObjectsOfType<AttackPattern>();

        for (int i = 0; i < attackPatterns.Length; i++)
        {
            // GameObject attackObj = CharacterBulletDatabase.GetForChangeAttackObject(attackPatterns[i].CharacterBulletIndex).gameObject;  

            // if (_newSprite != null && attackObj.GetComponent<SpriteRenderer>() != null)
            //     attackObj.GetComponent<SpriteRenderer>().sprite = _newSprite;

            // if (_addTrail)
            // {
            //     TrailRenderer tr = attackObj.AddComponent<TrailRenderer>();

            //     tr.time = _trailRenderer.time;
            //     tr.material = _trailRenderer.material;
            //     tr.widthCurve = _trailRenderer.widthCurve;
            //     tr.minVertexDistance = _trailRenderer.minVertexDistance;
            //     tr.widthMultiplier = _trailRenderer.widthMultiplier;
            //     tr.colorGradient = _trailRenderer.colorGradient;
            //     tr.numCornerVertices = _trailRenderer.numCornerVertices;
            // }

            // if (_modColor && attackObj.GetComponent<SpriteRenderer>() != null)
            // {
            //     attackObj.GetComponent<SpriteRenderer>().color = _bulletColor;
            // }

            // Transform halo = attackObj.transform.Find(halo_name);

            // if (_modHaloColor && halo != null)
            // {
            //     halo.GetComponent<SpriteRenderer>().color = _haloColor;
            // }

            // if (_modScale)
            // {
            //     attackObj.transform.localScale = new Vector3(attackObj.transform.localScale.x * _scaleMultiplier, attackObj.transform.localScale.y * _scaleMultiplier, 1f);
            // }
            // //attackObj.GetComponent<AttackObject>().Damage = Mathf.CeilToInt(attackObj.GetComponent<AttackObject>().Damage * _damageMult);

        }
    }
}
