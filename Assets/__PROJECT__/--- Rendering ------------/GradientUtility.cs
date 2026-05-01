using UnityEngine;

public static class GradientUtility
{
    public static Gradient RecolorGradientKeepBrightness(Gradient source, Color targetColor)
    {
        if (source == null)
            return null;

        GradientColorKey[] sourceColorKeys = source.colorKeys;
        GradientAlphaKey[] sourceAlphaKeys = source.alphaKeys;

        GradientColorKey[] newColorKeys = new GradientColorKey[sourceColorKeys.Length];

        Color.RGBToHSV(targetColor, out float targetH, out float targetS, out _);

        for (int i = 0; i < sourceColorKeys.Length; i++)
        {
            Color sourceColor = sourceColorKeys[i].color;

            Color.RGBToHSV(sourceColor, out _, out _, out float sourceV);

            Color recolored = Color.HSVToRGB(targetH, targetS, sourceV);
            recolored.a = sourceColor.a; // на всякий случай, хотя альфа у color key обычно не главная

            newColorKeys[i] = new GradientColorKey(recolored, sourceColorKeys[i].time);
        }

        Gradient result = new Gradient();
        result.SetKeys(newColorKeys, sourceAlphaKeys);

        return result;
    }
}