using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    public static Vector3 Bezier(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * control + Mathf.Pow(t, 2) * end;
    }

    public static Tween BezierFlyToTween(this Transform transform, Vector3 target, float duration, Vector3? control = null)
    {
        if (control == null)
            control = transform.position + (target - transform.position) * 0.5f + Vector3.up * Mathf.Sqrt(Vector3.Distance(transform.position, target));

        Vector3 start = transform.position;  // closure
        return DOVirtual.Float(0, 1, duration, t => transform.position = Bezier(start, control.Value, target, t))
            .SetEase(Ease.Linear);
    }

    public static float CalculateBezierDistance(Vector3 start, Vector3 control, Vector3 end, int segments = 100)
    {
        float distance = 0f;
        Vector3 previousPoint = start;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = Bezier(start, control, end, t);
            distance += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return distance;
    }

    public static Tween BezierFlyToTweenWithSpeed(this Transform transform, Vector3 target, float speed, Vector3? control = null)
    {
        if (control == null)
            control = transform.position + (target - transform.position) * 0.5f + Vector3.up * Mathf.Sqrt(Vector3.Distance(transform.position, target));

        float distance = CalculateBezierDistance(transform.position, control.Value, target);
        float duration = distance / speed;
        Vector3 start = transform.position;  // closure

        Debug.Log($"Distance: {distance}, Duration: {duration}");

        return DOVirtual.Float(0, 1, duration, t => transform.position 
            = Bezier(start, control.Value, target, t))
            .SetEase(Ease.Linear);
    }


    // Helper function to normalize angles within [0, 360)
    public static float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return angle;
    }


    public static float GetAngleDifference(float angle1, float angle2)
    {
        // Normalize both angles to be within 0° and 360°
        angle1 = angle1 % 360;
        angle2 = angle2 % 360;

        // Calculate the difference
        float angleDifference = angle2 - angle1;

        // Wrap the result to be between -360° and 360°
        if (angleDifference > 180)
            angleDifference -= 360;
        else if (angleDifference < -180)
            angleDifference += 360;

        return angleDifference;
    }

    public static float FindFarthestPoint(List<float> angles)
    {
        angles = angles.Select(a => a * Mathf.Rad2Deg).ToList();

        // Normalize all angles to the range [0, 360)
        for (int i = 0; i < angles.Count; i++)
        {
            angles[i] = NormalizeAngle(angles[i]);
        }

        // Sort the angles
        angles.Sort();

        foreach (var a in angles)
            Debug.Log(a);

        // Find the largest angular gap
        float largestGap = 0f;
        float bestAngle = 0f;

        for (int i = 0; i < angles.Count; i++)
        {
            // Calculate gap between consecutive angles
            float currentAngle = angles[i];
            float nextAngle = angles[(i + 1) % angles.Count]; // Circular comparison
            float gap = nextAngle - currentAngle;

            // Handle wrap-around between last and first angles
            if (i == angles.Count - 1)
            {
                nextAngle += 360f;
                gap = nextAngle - currentAngle;
            }

            // Check if this is the largest gap
            if (gap > largestGap)
            {
                largestGap = gap;
                bestAngle = (currentAngle + (gap / 2)) % 360; // Midpoint of the largest gap
            }
        }


        return bestAngle; // Angle farthest from all others
    }

}