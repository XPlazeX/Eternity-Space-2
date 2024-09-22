using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSkin : MonoBehaviour
{
    [SerializeField] private Sprite[] _skins;

    private void Start() {
        GetComponent<SpriteRenderer>().sprite = _skins[Skins.SOCurrentSkin()];
    }
}
