using UnityEngine;

public static class MathExtensions
{
    public static float Range(this System.Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static int Range(this System.Random random, int min, int max)
    {
        return random.Next(min, max);
    }

    public static float WithVariation(this float value, float variation)
    {
        return value + Random.Range(-variation, variation);
    }


    public static float LinearMap(float value, float a, float b, float c, float d, bool clamp=true)
    {
        if (clamp)
        {
            if (value <= a)
                return c;
            if (value >= b)
                return d;
        }

        return c + (value - a) * (d - c) / (b - a);
    }

}