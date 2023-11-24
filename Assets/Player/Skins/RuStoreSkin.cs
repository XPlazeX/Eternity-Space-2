using UnityEngine;

public class RuStoreSkin : MonoBehaviour
{
    [SerializeField] private Sprite _ruStoreSprite;

    private void Start() {
        if (Dev.RuStoreVersionSprites)
            GetComponent<SpriteRenderer>().sprite = _ruStoreSprite;
    }
}
