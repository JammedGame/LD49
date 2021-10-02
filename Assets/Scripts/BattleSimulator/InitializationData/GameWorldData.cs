using System;
using System.Collections.Generic;
using Game.Simulation.Board;
using Physics2D;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable]
	[CreateAssetMenu]
	public class GameWorldData : ScriptableObject
	{
		public BoardData Board;
		public GamePhysicsSettings PhysicsSettings;

		[Table]
		public List<UnitSpawn> UnitSpawns;
	}

	[Serializable]
	public struct UnitSpawn
	{
		public BattleObjectSettings Type;
		public Vector2 Position;
		public OwnerId Owner;

		public void Execute(GameWorld world)
		{
			Type?.Spawn(world, Position, Owner);
		}
	}

	[Serializable]
	public class GamePhysicsSettings : PhysicsSettings
	{
	}
}