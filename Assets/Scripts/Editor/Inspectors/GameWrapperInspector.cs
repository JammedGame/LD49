using System.Linq;
using Game;
using Game.Simulation;
using Game.Simulation.Board;
using Game.View;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameWrapper), true)]
public class GameWrapperInspector : Editor<GameWrapper>
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}

	void OnSceneGUI()
	{
		foreach (var polygon in target.GameData.Board.Polygons)
		{
			if (polygon == null || polygon.Points == null || polygon.Points.Length < 3)
				return;

			Handles.color = Color.white.WithAlpha(0.25f);
			UnityEditor.Handles.DrawAAConvexPolygon(polygon.Points.Select(x => ViewUtil.ConvertTo3D(x)).ToArray());

			for (int i = 0; i < polygon.Points.Length; i++)
			{
				var point3D = ViewUtil.ConvertTo3D(polygon.Points[i]);
				Handles.color = Color.red;
				point3D = UnityEditor.Handles.FreeMoveHandle(point3D, Quaternion.identity, HandleUtility.GetHandleSize(point3D) * 0.1f, default, Handles.CubeHandleCap);
				polygon.Points[i] = ViewUtil.ConvertTo2D(point3D);
			}
		}
	}
}