using Game.Simulation;
using Game.View;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitView), true)]
public class UnitViewInspector : Editor<UnitView>
{
	Editor settingsEditor;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		UnitSettings settings = target.Data is Unit unit
			? unit.Settings
			: Resources.Load<UnitSettings>("Settings/UnitSettings/" + target.name.Replace("View", ""));

		if (settings != null)
		{
			EditorGUILayout.Space();
			EditorGUILayout.ObjectField("Unit Settings", settings, typeof(UnitSettings), allowSceneObjects: false);
			EditorGUILayout.Space();
			Editor.DrawFoldoutInspector(settings, ref settingsEditor);
		}
	}
}