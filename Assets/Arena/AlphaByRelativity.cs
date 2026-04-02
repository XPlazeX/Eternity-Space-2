using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AlphaByRelativity : MonoBehaviour
{
    [SerializeField] private AnimationCurve alphaByRelativity;

    SpriteRenderer _sr;

    private void Start() {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, alphaByRelativity.Evaluate(PlayerController.AbsRelativity));
    }
}
