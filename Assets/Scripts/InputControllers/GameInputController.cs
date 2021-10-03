using System;
using BattleSimulator.Spells;
using Game.Simulation;
using Game.Simulation.Board;
using Game.View;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI
{
	public class GameInputController
	{
		private GameWrapper gameWrapper;
		public Unit SelectedUnit => gameWrapper.GameWorld.AllUnits[0];

		private BlightSpellSettings blightSpell;
		public GameInputController(GameWrapper gameWrapper)
		{
			this.gameWrapper = gameWrapper;
			blightSpell = Resources.Load("Settings/SpellSettings/Blight") as BlightSpellSettings;
			if (blightSpell == null)
			{
				Debug.LogError("Blight spell asset not found!");
			}
		}

		/// <summary>
		/// Reads user input and schedules actions.
		/// </summary>
		public void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				AttackMove();
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				CastSpell(blightSpell);
			}

			ControlUnitWASD(SelectedUnit);

			Time.timeScale = Input.GetKey(KeyCode.Alpha5) ? 4f : 1f;
		}

		private void ControlUnitWASD(Unit unit)
		{
			var isHoldingKeys = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");

			if (horizontal == 0 && vertical == 0)
			{
				if (unit.CurrentAction is UnitMoveWithDirectionAction)
				{
					unit.OrderIdle();
				}
			}
			else if (isHoldingKeys || unit.CurrentAction is UnitMoveWithDirectionAction)
			{
				var direction = math.mul(new float2(horizontal, vertical), float2x2.Rotate(gameWrapper.CameraController.Yaw * Mathf.Deg2Rad));
				var length = math.length(direction);
				if (length > 1) direction /= length;
				unit.OrderMoveInDirection(direction);
			}
		}

		private void AttackMove()
		{
			var raycastInfo = RaycastInfo.DoTheRaycast(gameWrapper, gameWrapper.Camera, Input.mousePosition);
			if (raycastInfo.TargetUnit != null) SelectedUnit.StartAttacking(new UnitTargetInfo(raycastInfo.TargetUnit));
			else SelectedUnit.OrderMoveToPoint(raycastInfo.TargetPosition);
		}

		private void CastSpell(SpellSettings spellSettings)
		{
			if (gameWrapper.TryMouseRaycast(out var hitResult))
			{
				var targetPosition = gameWrapper.GameWorld.Board.ClampPoint(hitResult);
				SelectedUnit.OrderSpellCast(spellSettings, targetPosition);
			}
		}
	}
}