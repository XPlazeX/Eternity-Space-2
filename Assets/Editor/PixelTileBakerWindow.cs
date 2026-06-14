using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PixelTileBakerWindow : EditorWindow
{
    [SerializeField] private Texture2D sourceTexture;
    [SerializeField] private PixelTilePalette palette;

    [SerializeField] private int tileSize = 8;
    [SerializeField] private bool ignoreAlpha = true;
    [SerializeField] private bool fillMissingPixelsWithOriginalColor = true;

    private class RuntimeTileRule
    {
        public Color32[] pixels;
        public bool allowRandomRotate;
        public bool allowFlipX;
        public bool allowFlipY;
    }

    [MenuItem("Tools/Pixel Tile Baker")]
    private static void Open()
    {
        GetWindow<PixelTileBakerWindow>("Pixel Tile Baker");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);

        sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Source Texture",
            sourceTexture,
            typeof(Texture2D),
            false
        );

        palette = (PixelTilePalette)EditorGUILayout.ObjectField(
            "Palette",
            palette,
            typeof(PixelTilePalette),
            false
        );

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        tileSize = EditorGUILayout.IntField("Tile Size", tileSize);
        ignoreAlpha = EditorGUILayout.Toggle("Ignore Alpha", ignoreAlpha);

        fillMissingPixelsWithOriginalColor = EditorGUILayout.Toggle(
            "Fill Missing With Color",
            fillMissingPixelsWithOriginalColor
        );

        EditorGUILayout.Space(10);

        using (new EditorGUI.DisabledScope(sourceTexture == null || palette == null))
        {
            if (GUILayout.Button("Generate PNG"))
            {
                Generate();
            }
        }
    }

    private void Generate()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is missing.");
            return;
        }

        if (palette == null)
        {
            Debug.LogError("Palette is missing.");
            return;
        }

        if (tileSize <= 0)
        {
            Debug.LogError("Tile size should be greater than zero.");
            return;
        }

        PrepareTextureForReading(sourceTexture);

        Dictionary<uint, RuntimeTileRule> tileMap = new();

        foreach (PixelTileRule rule in palette.rules)
        {
            if (rule.tile == null)
            {
                continue;
            }

            PrepareTextureForReading(rule.tile);

            if (rule.tile.width != tileSize || rule.tile.height != tileSize)
            {
                Debug.LogError(
                    $"Tile '{rule.tile.name}' has wrong size. " +
                    $"Expected {tileSize}x{tileSize}, got {rule.tile.width}x{rule.tile.height}."
                );

                return;
            }

            uint key = MakeColorKey(rule.color, ignoreAlpha);

            if (tileMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate color key in palette: {rule.color}");
            }

            tileMap[key] = new RuntimeTileRule
            {
                pixels = rule.tile.GetPixels32(),
                allowRandomRotate = rule.allowRandomRotate,
                allowFlipX = rule.allowFlipX,
                allowFlipY = rule.allowFlipY
            };
        }

        if (tileMap.Count == 0)
        {
            Debug.LogError("Palette has no valid tile rules.");
            return;
        }

        int sourceWidth = sourceTexture.width;
        int sourceHeight = sourceTexture.height;

        int outputWidth = sourceWidth * tileSize;
        int outputHeight = sourceHeight * tileSize;

        Color32[] sourcePixels = sourceTexture.GetPixels32();
        Color32[] outputPixels = new Color32[outputWidth * outputHeight];

        Color32 transparent = new Color32(0, 0, 0, 0);

        System.Random random = new System.Random();

        for (int sourceY = 0; sourceY < sourceHeight; sourceY++)
        {
            for (int sourceX = 0; sourceX < sourceWidth; sourceX++)
            {
                int sourceIndex = sourceY * sourceWidth + sourceX;
                Color32 sourceColor = sourcePixels[sourceIndex];

                uint key = MakeColorKey(sourceColor, ignoreAlpha);

                bool found = tileMap.TryGetValue(key, out RuntimeTileRule tileRule);

                int rotation = 0;
                bool flipX = false;
                bool flipY = false;

                if (found)
                {
                    if (tileRule.allowRandomRotate)
                    {
                        // 0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°
                        rotation = random.Next(0, 4);
                    }

                    if (tileRule.allowFlipX)
                    {
                        flipX = random.Next(0, 2) == 1;
                    }

                    if (tileRule.allowFlipY)
                    {
                        flipY = random.Next(0, 2) == 1;
                    }
                }

                for (int tileY = 0; tileY < tileSize; tileY++)
                {
                    for (int tileX = 0; tileX < tileSize; tileX++)
                    {
                        int outputX = sourceX * tileSize + tileX;
                        int outputY = sourceY * tileSize + tileY;

                        int outputIndex = outputY * outputWidth + outputX;

                        if (found)
                        {
                            outputPixels[outputIndex] = GetTransformedTilePixel(
                                tileRule.pixels,
                                tileSize,
                                tileX,
                                tileY,
                                rotation,
                                flipX,
                                flipY
                            );
                        }
                        else
                        {
                            outputPixels[outputIndex] = fillMissingPixelsWithOriginalColor
                                ? sourceColor
                                : transparent;
                        }
                    }
                }
            }
        }

        Texture2D outputTexture = new Texture2D(
            outputWidth,
            outputHeight,
            TextureFormat.RGBA32,
            false
        );

        outputTexture.SetPixels32(outputPixels);
        outputTexture.Apply(false, false);

        string savePath = EditorUtility.SaveFilePanelInProject(
            "Save Generated Texture",
            sourceTexture.name + "_Baked",
            "png",
            "Choose where to save generated PNG."
        );

        if (string.IsNullOrEmpty(savePath))
        {
            DestroyImmediate(outputTexture);
            return;
        }

        byte[] pngBytes = outputTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngBytes);

        DestroyImmediate(outputTexture);

        AssetDatabase.Refresh();

        Debug.Log($"Generated texture saved: {savePath}");
    }

    private static Color32 GetTransformedTilePixel(
        Color32[] pixels,
        int size,
        int outputX,
        int outputY,
        int rotation,
        bool flipX,
        bool flipY
    )
    {
        int x = outputX;
        int y = outputY;

        if (flipX)
        {
            x = size - 1 - x;
        }

        if (flipY)
        {
            y = size - 1 - y;
        }

        int sourceX;
        int sourceY;

        switch (rotation)
        {
            case 1:
                // 90°
                sourceX = y;
                sourceY = size - 1 - x;
                break;

            case 2:
                // 180°
                sourceX = size - 1 - x;
                sourceY = size - 1 - y;
                break;

            case 3:
                // 270°
                sourceX = size - 1 - y;
                sourceY = x;
                break;

            default:
                // 0°
                sourceX = x;
                sourceY = y;
                break;
        }

        return pixels[sourceY * size + sourceX];
    }

    private static uint MakeColorKey(Color32 color, bool ignoreAlpha)
    {
        byte alpha = ignoreAlpha ? (byte)255 : color.a;

        return
            ((uint)color.r << 24) |
            ((uint)color.g << 16) |
            ((uint)color.b << 8) |
            alpha;
    }

    private static void PrepareTextureForReading(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        bool changed = false;

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            changed = true;
        }

        if (importer.mipmapEnabled)
        {
            importer.mipmapEnabled = false;
            changed = true;
        }

        if (importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            changed = true;
        }

        if (importer.filterMode != FilterMode.Point)
        {
            importer.filterMode = FilterMode.Point;
            changed = true;
        }

        if (changed)
        {
            importer.SaveAndReimport();
        }
    }
}