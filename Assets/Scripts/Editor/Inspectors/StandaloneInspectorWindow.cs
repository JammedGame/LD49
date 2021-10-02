using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StandaloneInspectorWindow : EditorWindow
{
	private List<TargetObject> targetObjects;
	private Vector2 scrollArea;

	[MenuItem("Game/Inspect Game Data _F12", priority = 1001)]
	public static void ShowWindow()
	{
		GetWindow<StandaloneInspectorWindow>("Game Data");
	}

	public void OnGUI()
	{
		if (targetObjects == null)
		{
			targetObjects = new List<TargetObject>()
			{
				AssetDatabase.LoadMainAssetAtPath("Assets/Resources/Settings/UnitSettings/Paladin.asset"),
				AssetDatabase.LoadMainAssetAtPath("Assets/Resources/Settings/CameraSettings.asset"),
				AssetDatabase.LoadMainAssetAtPath("Assets/GameSettings/GameWorldData.asset"),
				AssetDatabase.LoadMainAssetAtPath("Assets/Scenes/Game_Profiles/Camera Profile.asset"),
			};
		}

		EditorGUIUtility.fieldWidth = 100;

		scrollArea = GUILayout.BeginScrollView(scrollArea);
		{
			EditorGUI.indentLevel = 1;
			foreach(var target in targetObjects)
			{
				target.Draw();
			}
		}
		GUILayout.EndScrollView();
	}

	public class TargetObject
	{
		public UnityEngine.Object targetObject;
		public UnityEditor.Editor editor;
		public bool folded;

		public void Draw()
		{
			if (targetObject == null)
				return;

			EditorGUILayout.BeginVertical();
			folded = EditorGUILayout.InspectorTitlebar(folded, targetObject);
			if (!folded)
			{
				if (targetObject is Material material)
				{
					UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObject, !folded);
					Editor.DrawFoldoutInspector(material, ref editor);
				}
				else
				{
					editor = editor ?? Editor.CreateEditor(targetObject);
					editor.OnInspectorGUI();
				}
			}
			EditorGUILayout.EndVertical();
		}

		public static implicit operator TargetObject(UnityEngine.Object obj)
		{
			if (obj == null)
			{
				Debug.LogError("TargetObject is null!");
			}

			return new TargetObject()
			{
				targetObject = obj,
				folded = true
			};
		}
	}
}
