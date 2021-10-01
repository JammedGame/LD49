using Game.Simulation;
using Physics2D;
using UnityEngine;

namespace Game.View.Debugging
{
	public static class UnitGizmos
	{
		[Gizmo]
		public static void DrawUnitViewGizmos(GameWrapper gameWrapper)
		{
			foreach(var unit in gameWrapper.GameWorld.AllUnits)
			{
				var unitView = gameWrapper.ViewController.GetView(unit);
				if (!unitView)
					continue;

				Gizmos.color = Color.white;
				Gizmos.matrix = Matrix4x4.TRS(unit.GetPosition3D(), unit.GetRotation3D(), new Vector3(unit.Radius, 0.1f, unit.Radius));
				Gizmos.DrawWireSphere(default, 1f);
			}
		}


		[Gizmo]
		public static void DrawProjectileGizmos(GameWrapper gameWrapper)
		{
			foreach (var projectile in gameWrapper.GameWorld.AllProjectiles)
			{
				Gizmos.color = projectile.IsActive ? Color.red : Color.black;
				Gizmos.DrawSphere(projectile.Position, 0.2f);
			}
		}
	}
}