using System.Collections.Generic;
using BattleSimulator.Spells;
using UnityEngine;

namespace Game.Simulation
{
	[CreateAssetMenu]
	public class UnitSettings : BattleObjectSettings
	{
		[Header("Movement")]
		public float Speed;

		[Header("Collision")]
		public float Size;
		public float Height = 1.5f;

		[Header("Health")]
		public bool IsInvulnerable;
		public float Health;

		[Header("Cast Upswing")]
		public float CastUpswing;

		[Header("Attack")]
		public UnitAttackAction PrimaryAttack;

		public List<SpellSettings> Spells;

		public UnitSettings SpawnOnDeath;

		public List<UnitSettings> SummoningList;

		public int GoldCost;

		public string UnitName;

		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return new Unit(gameWorld, this, targetInfo.Position, owner, parent);
		}

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(UnitName))
			{
				UnitName = name;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(this);
#endif
			}
		}
	}
}