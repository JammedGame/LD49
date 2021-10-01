using System.Linq;
using Game;
using Game.Simulation;
using Game.Simulation.Board;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameWrapper), true)]
public class GameWrapperInspector : Editor<GameWrapper>
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}