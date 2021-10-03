using System;
using System.Collections.Generic;
using Game.Simulation.Board;
using Physics2D;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    [Serializable]
    [CreateAssetMenu]
    public class GameWorldData : ScriptableObject
    {
        public int StartingGold = 250;
        public BoardData Board;
        public GamePhysicsSettings PhysicsSettings;

        [Table]
        public List<UnitSpawn> UnitSpawns;

        public List<WaveData> WaveData;
    }

    [Serializable]
    public struct UnitSpawn
    {
        public UnitSettings Type;
        public Vector2 Position;
        public OwnerId Owner;

        public Unit Execute(GameWorld world)
        {
			if (Type != null)
			{
				return world.SpawnUnit(Type, Position, Owner, null);
			}
			else
			{
				return null;
			}
		}
    }

    [Serializable]
    public class GamePhysicsSettings : PhysicsSettings
    {
    }
}