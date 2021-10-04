using BattleSimulator.Spells;
using Game.Simulation;
using Game.View;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI
{
	public class GameInputController
	{
		private GameWrapper gameWrapper;
		public Unit SelectedUnit => gameWrapper.GameWorld.AllUnits[0];
		private Unit selectedOther;

		private Unit player;

		private KeyCode[] spellKeys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};

		private readonly KeyCode[] summoningKeys =
			{ KeyCode.Alpha0, KeyCode.Alpha9, KeyCode.Alpha8, KeyCode.Alpha7, KeyCode.Alpha6, KeyCode.Alpha5 };

		private bool superSpeedEnabled;

		public GameInputController(GameWrapper gameWrapper)
		{
			this.gameWrapper = gameWrapper;

			player = gameWrapper.GameWorld.AllUnits[0];

			BlightSpellSettings blightSpell = Resources.Load("Settings/SpellSettings/Blight") as BlightSpellSettings;
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
			if (Input.GetMouseButtonDown(0))
			{
				ModifySelection();
			}

			if (Input.GetMouseButtonDown(1))
			{
				AttackMove();
			}

			for (int i = 0; i < spellKeys.Length; i++)
			{
				if (Input.GetKeyDown(spellKeys[i]))
				{
					CastSpell(i);
				}
			}

			for (var i = 0; i < summoningKeys.Length; i++)
			{
				if (Input.GetKeyDown(summoningKeys[i]))
				{
					Summon(i);
				}
			}


			ControlUnitWASD(SelectedUnit);

			if (Input.GetKeyDown(KeyCode.Alpha5)) ToggleSuperSpeed();
		}

		private void ToggleSuperSpeed()
		{
			superSpeedEnabled = !superSpeedEnabled;
			Time.timeScale = superSpeedEnabled ? 4f : 1f;
		}

		private void ControlUnitWASD(Unit unit)
		{
			if (unit.CurrentAction is CastSpellAction && !unit.CurrentActionExecuted)
			{
				return;
			}

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

		private void SelectUnit(Unit targetUnit)
		{
			var targetView = gameWrapper.ViewController.GetView(targetUnit);
			selectedOther = targetUnit;
			gameWrapper.ViewController.SelectObject(SelectedUnit.Owner, targetView);
			gameWrapper.GameWorld.UpdateSummoningList(selectedOther);
		}

		private void SelectUnitForAction(Unit targetUnit)
		{
			var targetView = gameWrapper.ViewController.GetView(targetUnit);
			selectedOther = targetUnit;
			gameWrapper.ViewController.SelectObjectForAction(SelectedUnit.Owner, targetView);
			gameWrapper.GameWorld.UpdateSummoningList(selectedOther);
		}

		private void UnselectUnit()
		{
			selectedOther = null;
			gameWrapper.ViewController.UnselectObject();
			gameWrapper.GameWorld.UpdateSummoningList(null);
		}

		private void ModifySelection()
		{
			var raycastInfo = RaycastInfo.DoTheRaycast(gameWrapper, gameWrapper.Camera, Input.mousePosition);
			var targetUnit = raycastInfo.TargetUnit;
			if (targetUnit != null) SelectUnit(targetUnit);
			else UnselectUnit();
		}

		private void AttackMove()
		{
			var raycastInfo = RaycastInfo.DoTheRaycast(gameWrapper, gameWrapper.Camera, Input.mousePosition);
			var targetUnit = raycastInfo.TargetUnit;
			if (targetUnit != null)
			{
				SelectUnitForAction(targetUnit);

				if (targetUnit.Owner != SelectedUnit.Owner && targetUnit.IsValidAttackTarget)
				{
					SelectedUnit.OrderAttacking(new UnitTargetInfo(targetUnit));
				}
				else
				{
					// todo jole: should be follow order
					SelectedUnit.OrderMoveToPoint(targetUnit.Position);
				}
			}
			else
			{
				UnselectUnit();

				var targetPosition = raycastInfo.TargetPosition;
				gameWrapper.ViewController.SelectPosition(ViewUtil.ConvertTo3D(targetPosition));
				SelectedUnit.OrderMoveToPoint(targetPosition);
			}
		}

		private void CastSpell(int index)
		{
			if (gameWrapper.TryMouseRaycast(out var hitResult))
			{
				var targetPosition = gameWrapper.GameWorld.Board.ClampPoint(hitResult);
				player.OrderSpellCast(index, targetPosition);
			}
		}

		private void Summon(int index)
		{
			var summoningList = selectedOther?.Settings.SummoningList;
			if (summoningList != null && summoningList.Count > 0)
			{
				if (index < 0 || index >= summoningList.Count) return;

				gameWrapper.GameWorld.SummonBuilding(summoningList[index], selectedOther);
				UnselectUnit();
			}
			else if (gameWrapper.TryMouseRaycast(out var hitResult))
			{
				summoningList = gameWrapper.GameWorld.Data.DefaultSummoningList;
				if (summoningList == null || index < 0 || index >= summoningList.Count) return;

				var targetPosition = gameWrapper.GameWorld.Board.ClampPoint(hitResult);
				gameWrapper.GameWorld.SummonCreep(summoningList[index], targetPosition, SelectedUnit.Owner);
			}
		}
	}
}