using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteUVRectBinder : MonoBehaviour
{
    private static readonly int SpriteUVRectId = Shader.PropertyToID("_SpriteUVRect");
    private static readonly int SpriteSizePixelsId = Shader.PropertyToID("_SpriteSizePixels");

    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _propertyBlock;

    private Sprite _lastSprite;
    private Texture _lastTexture;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();

        Apply();
    }

    private void OnValidate()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();

        Apply();
    }

    private void LateUpdate()
    {
        if (_spriteRenderer == null)
            return;

        Sprite sprite = _spriteRenderer.sprite;

        if (sprite != _lastSprite || sprite != null && sprite.texture != _lastTexture)
            Apply();
    }

    private void Apply()
    {
        if (_spriteRenderer == null)
            return;

        Sprite sprite = _spriteRenderer.sprite;

        if (sprite == null || sprite.texture == null)
            return;

        Texture texture = sprite.texture;
        Rect rect = sprite.textureRect;

        Vector4 uvRect = new Vector4(
            rect.x / texture.width,
            rect.y / texture.height,
            rect.width / texture.width,
            rect.height / texture.height
        );

        Vector4 spriteSizePixels = new Vector4(
            rect.width,
            rect.height,
            0f,
            0f
        );

        _spriteRenderer.GetPropertyBlock(_propertyBlock);

        _propertyBlock.SetVector(SpriteUVRectId, uvRect);
        _propertyBlock.SetVector(SpriteSizePixelsId, spriteSizePixels);

        _spriteRenderer.SetPropertyBlock(_propertyBlock);

        
    }
}