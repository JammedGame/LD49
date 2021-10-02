using Game.Simulation;
using Game.View;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingView), true)]
public class BuildingViewInspector : Editor<BuildingView>
{
	private Editor settingsEditor;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var settings = target.Data is Building building
			? building.Settings
			: Resources.Load<BuildingSettings>("Settings/BuildingSettings/" + target.name.Replace("View", ""));

		if (settings != null)
		{
			EditorGUILayout.Space();
			EditorGUILayout.ObjectField("Building Settings", settings, typeof(BuildingSettings), false);
			EditorGUILayout.Space();
			DrawFoldoutInspector(settings, ref settingsEditor);
		}
	}
}