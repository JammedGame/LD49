using BattleSimulator.Brains;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class Building : Unit
	{
		public new BuildingSettings Settings => (BuildingSettings)base.Settings;
		public override bool IsStatic => true;

		public Building(GameWorld gameWorld, BuildingSettings settings, float2 position, OwnerId owner, BattleObject parent)
			: base(gameWorld, settings, position, owner, parent)
		{
			SetBrain(new HoldGroundBrain());
		}

		public override string ViewPath => $"View/BuildingViews/{Settings.name}View";
	}
}