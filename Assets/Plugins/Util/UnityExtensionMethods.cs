using UnityEngine;

public static class ColorUtil
{
	public static Color WithAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
}