using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Pixel Tile Palette")]
public class PixelTilePalette : ScriptableObject
{
    public List<PixelTileRule> rules = new();
}

[Serializable]
public class PixelTileRule
{
    public Color32 color = new Color32(255, 255, 255, 255);
    public Texture2D tile;

    [Header("Random Transform")]
    public bool allowRandomRotate;
    public bool allowFlipX;
    public bool allowFlipY;
}