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

			var threshold = 100f * (Screen.height / 800f);
			var allUnits = wrapper.GameWorld.AllUnits;
			foreach (var unit in allUnits)
			{
				if (spell != null && unit is Creep creep && !spell.ShouldCreepBeIncludedInTargets(creep))
					continue;

				var pos1 = camera3D.WorldToScreenPoint(unit.GetCenterPosition3D());
				var diff1 = new Vector2(pos1.x - mousePosition.x, (pos1.y - mousePosition.y) * 0.85f);
				var pos2 = camera3D.WorldToScreenPoint(unit.GetPosition3D());
				var diff2 = new Vector2(pos2.x - mousePosition.x, (pos2.y - mousePosition.y) * 0.85f);
				var dist = Mathf.Min(diff1.magnitude, diff2.magnitude);
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

			return nearestUnit;
		}
	}
}