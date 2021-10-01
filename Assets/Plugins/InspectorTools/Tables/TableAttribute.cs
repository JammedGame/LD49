using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Put this to a List<T>/T[] field of some custom serializable class T for magic
/// Doesn't support nesting of lists.
/// </summary>
public class TableAttribute : PropertyAttribute { }

public class TableMenuItem : Attribute
{
	private static readonly object[] argList = { };

	public string Name;
	public MethodInfo Method;

	public TableMenuItem(string name)
	{
		Name = name;
	}

	public void Invoke(object target)
	{
		if (Method == null) return;
		Method.Invoke(target, argList);
	}
}

public class HideColumn : CellAttribute
{
	public HideColumn(params string[] hiddenFields) : base(hiddenFields) { }
}

/// <summary>
/// Use this if you want custom width of some column.
/// </summary>
public class CellWidth : PropertyAttribute
{
	public readonly float Width;
	public CellWidth(float width) { Width = width; }

	public class Big : CellWidth { public Big() : base(250f) { } }
	public class Medium : CellWidth { public Medium() : base(120f) { } }
	public class Small : CellWidth { public Small() : base(80f) { } }
	public class Tiny : CellWidth { public Tiny() : base(65f) { } }
}

/// <summary>
/// Returns error message, if there is some.
/// </summary>
public class ValidateCellAttribute : ActiveCellAttribute
{
	public ValidateCellAttribute(params string[] fields) : base(fields) { }
	public string Validate(object target) { return (string)Invoke(target); }
}

/// <summary>
/// Returns false if some cells should be hidden.
/// </summary>
public class CellVisibility : ActiveCellAttribute
{
	public CellVisibility(params string[] fields) : base(fields) { }
	public bool ShouldShow(object target) { return (bool)Invoke(target); }
}

/// <summary>
/// Cell attribute which invokes a method - result depends on list element.
/// </summary>
public class ActiveCellAttribute : CellAttribute
{
	public MethodInfo Method;
	private static readonly object[] argList = { };

	public ActiveCellAttribute(params string[] fields) : base(fields) { }

	public object Invoke(object target)
	{
		return Method != null ? Method.Invoke(target, argList) : null;
	}
}

/// <summary>
/// Cell attribute which can apply to some fields.
/// </summary>
public class CellAttribute : Attribute
{
	protected readonly List<string> FieldNames;

	public CellAttribute(params string[] fields)
	{
		FieldNames = new List<string>(fields);
	}

	public bool AppliesTo(string fieldName)
	{
		return FieldNames.Contains(fieldName);
	}
}