using UnityEngine;

public static class ParticleSystemLifetimeUtility
{
    public static float GetRootDuration(ParticleSystem ps)
    {
        if (ps == null)
            return 0f;

        var main = ps.main;

        if (main.loop)
            return Mathf.Infinity;

        float startDelay = GetMax(main.startDelay);
        float startLifetime = GetMax(main.startLifetime);

        return startDelay + main.duration + startLifetime;
    }

    private static float GetMax(ParticleSystem.MinMaxCurve curve)
    {
        switch (curve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return curve.constant;

            case ParticleSystemCurveMode.TwoConstants:
                return Mathf.Max(curve.constantMin, curve.constantMax);

            case ParticleSystemCurveMode.Curve:
                return GetMaxFromCurve(curve.curve) * curve.curveMultiplier;

            case ParticleSystemCurveMode.TwoCurves:
                float minMax = GetMaxFromCurve(curve.curveMin);
                float maxMax = GetMaxFromCurve(curve.curveMax);
                return Mathf.Max(minMax, maxMax) * curve.curveMultiplier;

            default:
                return 0f;
        }
    }

    private static float GetMaxFromCurve(AnimationCurve curve)
    {
        if (curve == null || curve.length == 0)
            return 0f;

        float max = float.MinValue;

        for (int i = 0; i < curve.length; i++)
            max = Mathf.Max(max, curve.keys[i].value);

        return max;
    }
}