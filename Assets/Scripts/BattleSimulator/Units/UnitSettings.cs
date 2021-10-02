using System;
using System.Collections.Generic;
using BattleSimulator.Spells;
using Physics2D;
using Unity.Mathematics;
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

		[Header("Health")]
		public float Health;

		[Header("Cast Upswing")]
		public float CastUpswing;

		[Header("Attack")]
		public UnitAttackAction PrimaryAttack;

		[Header("Spells")]
		public List<SpellSettings> Spells;

		public override BattleObject Spawn(GameWorld world, float2 position, OwnerId owner, BattleObject parent)
		{
			return world.SpawnUnit(this, position, owner);
		}
	}
}