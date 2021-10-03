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
			var nearest = 30f;
			var allUnits = wrapper.GameWorld.AllUnits;
			foreach (var unit in allUnits)
			{
				if (spell != null && unit is Creep creep && !spell.ShouldCreepBeIncludedInTargets(creep)) continue;

				var pos = camera3D.WorldToScreenPoint(unit.GetPosition3D() + Vector3.up * 0.5f);
				var diff = new Vector2(pos.x - mousePosition.x, (pos.y - mousePosition.y) * 0.85f);
				var dist = diff.magnitude;
				if (dist < nearest)
				{
					nearest = dist;
					nearestUnit = unit;
				}
			}

			return nearestUnit;
		}
	}
}