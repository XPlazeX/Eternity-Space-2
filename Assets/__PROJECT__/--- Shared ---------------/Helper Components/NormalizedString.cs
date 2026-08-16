using UnityEngine;

public static class NormalizedString
{
    public static string Convert(float n, NormalizedValueFormat format)
    {
        switch (format)
        {
            case NormalizedValueFormat.Raw00:
                return Mathf.Clamp01(n).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

            case NormalizedValueFormat.Bool01:
                return Mathf.Clamp01(n) == 1f ? "1" : "0";

            case NormalizedValueFormat.Percents100:
                return Mathf.RoundToInt(Mathf.Clamp01(n) * 100f).ToString();

            case NormalizedValueFormat.RawInt:
                return Mathf.CeilToInt(n).ToString();

            case NormalizedValueFormat.Raw:
            default:
                return n.ToString();
        }
    }
}

public enum NormalizedValueFormat
{
    Raw, // 0.7981478979857297489
    RawInt,
    Raw00, // 0.95
    Bool01, // 0...1
    Percents100 // 65%
}