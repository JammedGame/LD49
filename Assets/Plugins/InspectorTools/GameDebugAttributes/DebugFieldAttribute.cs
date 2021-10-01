using System;

public class DebugRangeAttribute : DebugFieldAttribute
{
	public float minValue;
	public float maxValue;

	public DebugRangeAttribute(float v1, float v2)
	{
		this.minValue = v1;
		this.maxValue = v2;
	}
}

public class DebugFieldAttribute : System.Attribute
{
}