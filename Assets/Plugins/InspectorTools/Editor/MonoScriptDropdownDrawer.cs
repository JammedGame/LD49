using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MonoScriptDropdown), true)]
public class MonoScriptDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		DrawScriptDropwdownInternal(position, property.serializedObject);
	}

	public static bool Draw(SerializedObject serializedObject)
	{
		var position = EditorGUILayout.GetControlRect();
		return DrawScriptDropwdownInternal(position, serializedObject);
	}

	private static bool DrawScriptDropwdownInternal(Rect position, SerializedObject serializedObject)
	{
		// get root type
		var rootType = serializedObject.targetObject.GetType();
		while (true)
		{
			if (rootType.BaseType == null || rootType.BaseType.AssemblyQualifiedName.Contains("Unity")) break;
			rootType = rootType.BaseType;
		}

		// get list of types
		var typeNames = new List<Type>();
		typeNames.Add(rootType);
		typeNames.AddRange(TypeCache.GetTypesDerivedFrom(rootType));
		typeNames.RemoveAll(x => x.IsAbstract);

		// get options
		var options = typeNames.Select(x => x.Name).ToArray();
		var oldIndex = typeNames.IndexOf(serializedObject.targetObject.GetType());

		// draw actual dropdown
		var newIndex = EditorGUI.Popup(position, " ", oldIndex, options);
		if (newIndex != oldIndex)
		{
			var mScript = serializedObject.FindProperty("m_Script");
			var newName = options[newIndex] + ".cs";
			var newValue = AssetDatabase
				.FindAssets("t:script")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(x => x.EndsWith(newName))
				.Select(AssetDatabase.LoadMainAssetAtPath)
				.OfType<MonoScript>()
				.FirstOrDefault();

			if (newValue == null)
			{
				Debug.LogError($"Failed to find monoScript for: {options[newIndex]}");
			}
			else
			{
				mScript.objectReferenceValue = newValue;
				return serializedObject.ApplyModifiedProperties();
			}
		}

		return false;
	}
}