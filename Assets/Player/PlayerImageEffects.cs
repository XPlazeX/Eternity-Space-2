using UnityEngine;
using EffectsDesign;

[RequireComponent(typeof(PlayerDamageBody))]
public class PlayerImageEffects : MonoBehaviour
{
    private ParticleSystem _playerTrailPS;
    private Animator _playerTrailAnimator;

    private void Awake() 
    {
        _playerTrailPS = GetComponent<ParticleSystem>();
        _playerTrailAnimator = GetComponent<Animator>();

        _playerTrailPS.textureSheetAnimation.SetSprite(0, GetComponent<SpriteRenderer>().sprite);
    }

    private void OnEnable() {
        PlayerController.BeginDrag += Flash;
    }

    private void OnDisable() {
        PlayerController.BeginDrag -= Flash;
    }

    public void Flash()
    {
        _playerTrailAnimator.SetTrigger("Flash");
    }
}
