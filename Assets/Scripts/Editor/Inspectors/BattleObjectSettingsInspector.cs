using Game.Simulation;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BattleObjectSettings), true)]
public class BattleObjectSettingsInspector : Editor<BattleObjectSettings>
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}