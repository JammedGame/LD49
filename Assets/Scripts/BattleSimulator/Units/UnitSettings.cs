using System;
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

		[Header("Attack")]
		public UnitAttackAction PrimaryAttack;

		public override BattleObject Spawn(GameWorld world, float2 position, OwnerId owner)
		{
			return world.SpawnUnit(this, position, owner);
		}
	}
}