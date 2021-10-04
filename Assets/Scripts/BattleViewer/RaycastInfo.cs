using BattleSimulator.Spells;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace Game.View
{
	/// <summary>
	/// Class which contains the context of a battle camera raycast.
	/// </summary>
	public class RaycastInfo
	{
		public float2 TargetPosition { get; private set; }

		public Unit TargetUnit { get; private set; }

		/// <summary>
		/// Does the raycast and returns an instance of the BattleRaycastInfo.
		/// </summary>
		public static RaycastInfo DoTheRaycast(GameWrapper wrapper, Camera camera3D, Vector2 mousePosition, Spell spell = null)
		{
			var newRaycast = new RaycastInfo();

			// Check if unit has been clicked
			if (spell == null || spell.IsSingleTarget)
			{
				newRaycast.TargetUnit = GetNearestUnit(wrapper, camera3D, mousePosition, spell);
				if (spell != null && newRaycast.TargetUnit == null) // no creep found which satisfies spell's criteria - try spell agnostic check to see which creep dummy user tried to cast on
				{
					newRaycast.TargetUnit = GetNearestUnit(wrapper, camera3D, mousePosition);
				}

				if (newRaycast.TargetUnit != null)
				{
					newRaycast.TargetPosition = newRaycast.TargetUnit.Position;
					return newRaycast;
				}
			}

			// Check if map has been clicked.
			if (wrapper.TryMouseRaycast(out var hitResult))
			{
				newRaycast.TargetPosition = wrapper.GameWorld.Board.ClampPoint(hitResult);
			}

			return newRaycast;
		}

		private static Unit GetNearestUnit(GameWrapper wrapper, Camera camera3D, Vector2 mousePosition, Spell spell = null)
		{
			var nearestUnit = default(Unit);
			var nearestUnitDist = float.MaxValue;

			var threshold = 1.01f;
			var allUnits = wrapper.GameWorld.AllUnits;
			var ray = camera3D.ScreenPointToRay(mousePosition);
			var startPoint = ray.origin;
			var endPoint = ray.origin + ray.direction * 200;
			foreach (var unit in allUnits)
			{
				if (spell != null && unit is Creep creep && !spell.ShouldCreepBeIncludedInTargets(creep))
					continue;

				Vector3 point = unit.GetCenterPosition3D();
				var dist = MathUtil.GetDistanceToLine(point, startPoint, endPoint, unit.Settings.Height, Mathf.Max(1f, unit.Radius));
				if (dist < threshold)
				{
					// prefer enemies for raycast.
					if (nearestUnit == null
					|| (nearestUnit.Owner == OwnerId.Player1 && unit.Owner == OwnerId.Player2)
					|| (nearestUnit.Owner == unit.Owner && dist < nearestUnitDist))
					{
						nearestUnit = unit;
						nearestUnitDist = dist;
					}
				}
			}

			Debug.Log(nearestUnit);
			return nearestUnit;
		}
	}
}