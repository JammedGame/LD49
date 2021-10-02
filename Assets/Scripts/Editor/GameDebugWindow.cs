using System.Linq;
using System.Reflection;
using Game;
using UnityEditor;
using UnityEngine;

public class GameDebugWindow : EditorWindow
{
	[MenuItem("Game/Debug _%g")]
	static new GameDebugWindow Show() => EditorWindow.GetWindow<GameDebugWindow>();

	public void OnGUI()
	{
		DrawGizmosGUI();
	}

	public static void DrawGizmosGUI()
	{
		EditorGUI.indentLevel = 1;
		EditorGUILayout.Space();
		EditorGUIUtility.labelWidth = Mathf.Min(300, Screen.width * 0.4f);

		EditorGUILayout.LabelField("Gizmos", EditorStyles.boldLabel);
		foreach (var gizmo in GizmoService.GetAllGizmoMethods())
		{
			var isEnabled = gizmo.IsEnabledPref.GetValue();
			var newIsEnabled = EditorGUILayout.Toggle(gizmo.methodInfo.Name, isEnabled);
			if (newIsEnabled != isEnabled)
			{
				gizmo.IsEnabledPref.SetValue(newIsEnabled);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Debug Fields", EditorStyles.boldLabel);

		foreach(var field in TypeCache.GetFieldsWithAttribute<DebugFieldAttribute>().Where(x => x.IsStatic))
		{
			DrawStaticField(field);
		}
	}

	private static void DrawStaticField(FieldInfo field)
	{
		if (field == null)
			return;

		var debugFieldAttribute = field.GetCustomAttribute<DebugFieldAttribute>();

		if (field.FieldType == typeof(float))
		{
			string name = ObjectNames.NicifyVariableName(field.Name);
			float oldValue = (float)field.GetValue(null);
			float newValue = debugFieldAttribute is DebugRangeAttribute range
				? EditorGUILayout.Slider(name, oldValue, range.minValue, range.maxValue)
				: EditorGUILayout.FloatField(name, oldValue);
			if (newValue != oldValue)
			{
				field.SetValue(null, newValue);
				OnFieldModified();
			}
		}
		else if (field.FieldType == typeof(bool))
		{
			string name = ObjectNames.NicifyVariableName(field.Name);
			bool oldValue = (bool)field.GetValue(null);
			bool newValue = EditorGUILayout.Toggle(name, oldValue);
			if (newValue != oldValue)
			{
				field.SetValue(null, newValue);
				OnFieldModified();
			}
		}
		else if (field.FieldType == typeof(int))
		{
			string name = ObjectNames.NicifyVariableName(field.Name);
			int oldValue = (int)field.GetValue(null);
			int newValue = EditorGUILayout.IntField(name, oldValue);
			if (newValue != oldValue)
			{
				field.SetValue(null, newValue);
				OnFieldModified();
			}
		}
	}

	private static void OnFieldModified()
	{
	}
}