using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public abstract class BattleObjectSettings : ScriptableObject
	{
		public abstract BattleObject Spawn(GameWorld world, float2 position, OwnerId owner);
	}
}