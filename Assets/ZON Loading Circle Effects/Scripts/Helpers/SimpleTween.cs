using UnityEngine;

public class SimpleTween{
    public static float EaseOutQuat(float currentTime, float startNumber, float endNumber, float duration)
	{
		if (duration == 0) {
			return endNumber;
		}

        float t = currentTime / duration;
        float changingAmount = endNumber - startNumber;
        return -changingAmount * t * (t - 2) + startNumber;
    }
    public static Color EaseOutQuat(float currentTime, Color startColor, Color endColor, float duration)
    {
        Color currentColor = startColor;

        currentColor.r = EaseOutQuat(currentTime, startColor.r, endColor.r, duration);
        currentColor.g = EaseOutQuat(currentTime, startColor.g, endColor.g, duration);
        currentColor.b = EaseOutQuat(currentTime, startColor.b, endColor.b, duration);
        currentColor.a = EaseOutQuat(currentTime, startColor.a, endColor.a, duration);

        return currentColor;
    }

    public static float EaseInQuat(float currentTime, float startNumber, float endNumber, float duration)
    {
		if (duration == 0) {
			return endNumber;
		}

        float t = currentTime / duration;
        float changingAmount = endNumber - startNumber;
        return changingAmount * t * t  + startNumber;
    }

    public static float Linear(float currentTime, float startNumber, float endNumber, float duration)
	{
		if (duration == 0) {
			return endNumber;
		}

        float t = currentTime / duration;
        float changingAmount = endNumber - startNumber;
        return changingAmount * t + startNumber;
    }

    public static Color Linear(float currentTime, Color startColor, Color endColor, float duration)
	{
		if (duration == 0) {
			return endColor;
		}

        float t = currentTime / duration;
        return Color.Lerp(startColor, endColor, t);
    }
}
