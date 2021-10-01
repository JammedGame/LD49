using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;

[CustomPropertyDrawer(typeof(TableAttribute))]
public class TablePropertyDrawer : PropertyDrawer
{
	private bool initialized;
	private Type elementType;
	private int elementIndex;

	private readonly List<TableMenuItem> tableMenuItems = new List<TableMenuItem>();

	/// <summary>
	/// PropertyDrawers are called once per lists/array element, not once per list.
	/// </summary>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// reflection stuff, cached for performance
		if (!initialized)
		{
			Initialize(property);
			initialized = true;
		}

		// check if this is assigned to list/array of custom serialized class/struct
		var list = property.GetValue<IList>();
		if (list == null || elementType == null)
		{
			// no need for sub properties, fallback to normal property drawer.
			EditorGUI.PropertyField(position, property, label);
			return;
		}

		// get and cache property path for property that represents start of the next row.
		var endProperty = property.GetEndProperty(true);
		var endPropertyPath = endProperty != null ? endProperty.propertyPath : "";

		// draw header if first element of the list.
		Rect rowRect = position;
		rowRect.height = BaseHeight;

		if (IsFirstElement(property))
		{
			elementIndex = 0;
			var headerRect = position;
			headerRect.y += 10f;
			headerRect.height = BaseHeight;
			{
				var headerProp = property.Copy();
				for (headerProp.Next(true); headerProp.propertyPath != endPropertyPath; headerProp.Next(false))
				{
					var columnInfo = GetColumnInfo(headerProp);
					if (columnInfo.Hidden) { continue; }
					headerRect.width = columnInfo.Width;
					GUI.Label(headerRect, new GUIContent(headerProp.displayName, headerProp.tooltip), CenteredLabel);
					headerRect.x += headerRect.width - 10f;
				}
			}

			// move rect a bit lower.
			rowRect.y = headerRect.y + BaseHeight;
		}

		// Right click options
		if (Event.current.type == EventType.ContextClick && position.Contains(Event.current.mousePosition))
		{
			ShowRightClickMenu(property.serializedObject, property, elementIndex, list);
		}

		// Draw one row
		string errorMessage = null;
		{
			// Context menu buttons
			rowRect.x -= 10f;
			rowRect.width = 20f;
			if (GUI.Button(rowRect, "", CenteredLabel)) { ShowRightClickMenu(property.serializedObject, property, elementIndex, list); }

			// Draw cells
			for (property.Next(true); property.propertyPath != endPropertyPath; property.Next(false))
			{
				var columnInfo = GetColumnInfo(property);
				if (columnInfo.Hidden) { continue; }

				if (list.Count > elementIndex && columnInfo.ShowCell(list[elementIndex]))
				{
					string validationMessage;
					if (!columnInfo.Validate(list[elementIndex], out validationMessage))
					{
						errorMessage = validationMessage;
						GUI.backgroundColor = ErrorColor;
					}
					rowRect.width = columnInfo.Width;
					DrawProperty(rowRect, property);
					rowRect.x += columnInfo.Width;
					GUI.backgroundColor = Color.white;
				}
				else
				{
					rowRect.x += columnInfo.Width;
				}
				rowRect.x -= 10f;
			}

			// Draw validation Message if necessary
			if (!string.IsNullOrEmpty(errorMessage)) 
			{ 
				rowRect.width = position.width;
				GUI.Label(rowRect, errorMessage, ErrorMessageStyle);
			}
		}

		// update list index for next property to use.
		if (elementIndex < list.Count - 1) { elementIndex++; }
	}

	/// <summary>
	/// Initializes stuff that requires reflection.
	/// </summary>
	private void Initialize(SerializedProperty property)
	{
		// check if element type exists and is not a primitive.
		if (!property.hasVisibleChildren) { return; }
		var genericArgs = fieldInfo.FieldType.GetGenericArguments();
		elementType = genericArgs.Length > 0 ? genericArgs[0] : fieldInfo.FieldType.GetElementType();
		if (elementType == null) { return; }

		// parse generic type methods, search for active cell attributes.
		var endPropertyPath = property.GetEndProperty().propertyPath;
		foreach (var method in elementType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
		{
			// if method has active attributes, find appropriate column and apply it.
			foreach (var activeAttribute in GetAttributes<ActiveCellAttribute>(method))
			{
				activeAttribute.Method = method;
				var headerProp = property.Copy();
				for (headerProp.Next(true); headerProp.propertyPath != endPropertyPath; headerProp.Next(false))
				{
					var column = GetColumnInfo(headerProp);
					if (activeAttribute.AppliesTo(headerProp.name)) { column.Add(activeAttribute); }
				}
			}

			foreach (var tableMenuItem in GetAttributes<TableMenuItem>(method))
			{
				tableMenuItem.Method = method;
				tableMenuItems.Add(tableMenuItem);
			}
		}
	}

	/// <summary>
	/// Draws context menu.
	/// </summary>
	private void ShowRightClickMenu(SerializedObject serializedObject, SerializedProperty listProperty, int elemIndex, IList list)
	{
		if (elemIndex > list.Count - 1 || elemIndex < 0) { return; }

		var path = listProperty.propertyPath;
		var listStart = path.LastIndexOf(".Array.", StringComparison.Ordinal);
		if (listStart < 0) { return; }
		path = path.Substring(0, listStart);
		listProperty = serializedObject.FindProperty(path);
		
		var menu = new GenericMenu();
		menu.AddItem(new GUIContent("Move Up"), false, () =>
		{
			if (elemIndex <= 0) { return; }
			listProperty.MoveArrayElement(elemIndex, elemIndex - 1);
			serializedObject.ApplyModifiedProperties();
		});

		menu.AddItem(new GUIContent("Move Down"), false, () =>
		{
			if (elemIndex >= list.Count - 1) { return; }
			listProperty.MoveArrayElement(elemIndex, elemIndex + 1);
			serializedObject.ApplyModifiedProperties();
		});

		menu.AddSeparator("");

		menu.AddItem(new GUIContent("Duplicate Row"), false, () =>
		{
			listProperty.InsertArrayElementAtIndex(elemIndex);
			serializedObject.ApplyModifiedProperties();
			listProperty.MoveArrayElement(elemIndex, elemIndex + 1);
			serializedObject.ApplyModifiedProperties();
		});

		menu.AddItem(new GUIContent("Delete Row"), false, () =>
		{
			listProperty.DeleteArrayElementAtIndex(elemIndex);
			serializedObject.ApplyModifiedProperties();
		});

		if (tableMenuItems.Count > 0)
		{
			menu.AddSeparator("");
			for (int i = 0; i < tableMenuItems.Count; i++)
			{
				var tableMenuItem = tableMenuItems[i];
				menu.AddItem(new GUIContent(tableMenuItem.Name), false, () =>
				{
					tableMenuItem.Invoke(list[elemIndex]);
				});
			}
		}

		menu.ShowAsContext();
		Event.current.Use();
	}

	/// <summary>
	/// Cache of ColumnInfo objects.
	/// </summary>
	public ColumnInfo GetColumnInfo(SerializedProperty property)
	{
		ColumnInfo info;
		if (!cache.TryGetValue(property.name, out info))
		{
			info = new ColumnInfo(property, elementType, fieldInfo);
			cache.Add(property.name, info);
		}
		return info;
	}
	private readonly Dictionary<string, ColumnInfo> cache = new Dictionary<string, ColumnInfo>();

	/// <summary>
	/// All info about one column in the table.
	/// </summary>
	public class ColumnInfo
	{
		public readonly FieldInfo ColumnField;
		public readonly bool Hidden;
		public readonly float PixelWidth;
		public readonly float Width;

		public ColumnInfo(SerializedProperty property, Type genericType, FieldInfo listField)
		{
			// Get representing field.
			ColumnField = genericType.GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (ColumnField == null) { Debug.LogError("Failed to find field:" + property.name + " for class: " + genericType); return; }

			// Column params.
			PixelWidth = CalculateColumnWidth(property);
			Width = PixelWidth;

			// Should hide column?
			foreach (var hideColumn in GetAttributes<HideColumn>(listField))
				if (hideColumn.AppliesTo(property.name)) { Hidden = true; }
		}

		/// <summary>
		/// Returns width of the column in pixels.
		/// </summary>
		private float CalculateColumnWidth(SerializedProperty property)
		{
			// Check for user given value through CellWIdth attribute.
			foreach (var attribute in GetAttributes<CellWidth>(ColumnField))
				return attribute.Width;

			// get default width for property type.
			float width = 80f;
			switch (property.propertyType)
			{
				case SerializedPropertyType.Boolean: width = 40; break;
				case SerializedPropertyType.ObjectReference: width = 250; break;
				case SerializedPropertyType.String: width = 250; break;
				case SerializedPropertyType.Vector2: width = 150; break;
				case SerializedPropertyType.Vector3: width = 225; break;
				case SerializedPropertyType.Vector4: width = 240; break;
			}

			// adjust for long field/enum names
			var minWidth = CenteredLabel.CalcSize(new GUIContent(property.displayName)).x + 18f;
			if (ColumnField.FieldType.IsEnum)
			{
				foreach (var enumValue in Enum.GetValues(ColumnField.FieldType))
				{
					var enumLength = EditorStyles.popup.CalcSize(new GUIContent(ObjectNames.NicifyVariableName(enumValue.ToString()))).x + 18f;
					if (enumLength > minWidth) { minWidth = enumLength; }
				}
			}
			return Mathf.Max(minWidth, width);
		}

		#region Active cells

		public readonly List<CellVisibility> OptionalCell = new List<CellVisibility>();
		public readonly List<ValidateCellAttribute> ValidateCell = new List<ValidateCellAttribute>();

		public void Add(ActiveCellAttribute activeAttribute)
		{
			var validateCell = activeAttribute as ValidateCellAttribute;
			if (validateCell != null) ValidateCell.Add(validateCell);

			var optionalCell = activeAttribute as CellVisibility;
			if (optionalCell != null) OptionalCell.Add(optionalCell);
		}

		/// <summary>
		/// Returns true if cell should be shown, false if it should be hidden
		/// </summary>
		public bool ShowCell(object listElem)
		{
			foreach (var optionalCell in OptionalCell)
			{
				try
				{
					if (!optionalCell.ShouldShow(listElem)) return false;
				}
				catch (Exception e) { Debug.LogException(e); }
			}
			return true;
		}

		/// <summary>
		/// Returns true if object is valid, false and returns error message if not.
		/// </summary>
		public bool Validate(object listElem, out string errorMessage)
		{
			foreach (var validateCell in ValidateCell)
			{
				try
				{
					var validateMessage = validateCell.Validate(listElem);
					if (!string.IsNullOrEmpty(validateMessage))
					{
						errorMessage = validateMessage;
						return false;
					}
				}
				catch (Exception e) { Debug.LogException(e); }
			}
			errorMessage = null;
			return true;
		}

		#endregion
	}

	// Don't ask don't tell Unity magic
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
	{ 
		if (IsFirstElement(property)) return 2 * BaseHeight + 10f;
		if (IsLastElement(property)) return BaseHeight + 8f;
		return BaseHeight; 
	}

	public float BaseHeight { get { return EditorGUIUtility.singleLineHeight; } }

	// True if first element in list
	public bool IsFirstElement(SerializedProperty property)
	{
		return property.propertyPath.EndsWith(".Array.data[0]", StringComparison.Ordinal);
	}

	public bool IsLastElement(SerializedProperty property)
	{
		return property.isArray && property.propertyPath.EndsWith(".Array.data[" + (property.arraySize - 1).ToString() + "]", StringComparison.Ordinal);
	}

	/// <summary>
	/// Special drawing functions for some properties.
	/// </summary>
	public void DrawProperty(Rect rect, SerializedProperty property)
	{
		switch(property.propertyType)
		{
			case SerializedPropertyType.Boolean:

				// center layout
				var toggleRect = rect;
				toggleRect.width = 20f;
				toggleRect.center = rect.center;
				toggleRect.x += toggleRect.width;

				// apply if modified.
				GUI.changed = false;
				property.boolValue = GUI.Toggle(toggleRect, property.boolValue, Empty);
				if (GUI.changed) { property.serializedObject.ApplyModifiedProperties(); }

				return;

			default:
				EditorGUI.PropertyField(rect, property, Empty);
				return;
		}
	}

	// Cached label style
	private static readonly GUIStyle CenteredLabel;
	private static readonly GUIStyle ErrorMessageStyle;
	private static readonly GUIContent Empty = new GUIContent();
	private static readonly Color ErrorColor = new Color(1f, 0.16f, 0.16f);
	static TablePropertyDrawer()
	{
		CenteredLabel = new GUIStyle(GUI.skin.label);
		CenteredLabel.alignment = TextAnchor.MiddleCenter;

		ErrorMessageStyle = new GUIStyle(GUI.skin.label);
		ErrorMessageStyle.normal.textColor = ErrorColor;
		ErrorMessageStyle.alignment = TextAnchor.MiddleRight;
	}

	public static T[] GetAttributes<T>(MemberInfo field)
	{
		return Array.ConvertAll(field.GetCustomAttributes(typeof(T), true), a => (T)a);
	}
}