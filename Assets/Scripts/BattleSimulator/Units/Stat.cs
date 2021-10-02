using UnityEngine;
using static Unity.Mathematics.math;

[System.Serializable]
public struct Stat
{
	/* base value */
	[SerializeField] float baseValue;

	/* applied modifiers */
	float incFlat;
	float incPercent;
	float decreaseRelative;

	/* calculated value */
	float value;

	void RecalculateValue()
	{
		value = (baseValue * (1 + incPercent) + incFlat) * (1 - decreaseRelative);
	}

	public void IncFlat(float value)
	{
		incFlat += value;
		RecalculateValue();
	}

	public void IncPercent(float value)
	{
		incPercent += value;
		RecalculateValue();
	}

	public void Decrease(float value)
	{
		decreaseRelative = max(decreaseRelative, value);
		RecalculateValue();
	}

	public void Reset()
	{
		incFlat = 0;
		decreaseRelative = 0;
		incPercent = 0;
		value = baseValue;
	}

	public float Value => value;
	public float BaseValue => baseValue;

	public static implicit operator float(Stat x) => x.value;
	public static implicit operator Stat(float x) => new Stat() { baseValue = x, value = x };

	public static float operator +(Stat lhs, Stat rhs) => lhs.value + rhs.value;
	public static float operator +(Stat lhs, float rhs) => lhs.value + rhs;
	public static float operator +(float lhs, Stat rhs) => lhs + rhs.value;

	public static float operator -(Stat lhs, Stat rhs) => lhs.value - rhs.value;
	public static float operator -(Stat lhs, float rhs) => lhs.value - rhs;
	public static float operator -(float lhs, Stat rhs) => lhs - rhs.value;

	public static float operator *(Stat lhs, Stat rhs) => lhs.value * rhs.value;
	public static float operator *(Stat lhs, float rhs) => lhs.value * rhs;
	public static float operator *(float lhs, Stat rhs) => lhs * rhs.value;

	public static float operator /(Stat lhs, Stat rhs) => lhs.value / rhs.value;
	public static float operator /(Stat lhs, float rhs) => lhs.value / rhs;
	public static float operator /(float lhs, Stat rhs) => lhs / rhs.value;


	public override string ToString() => value.ToString();
}