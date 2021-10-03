using System;
using BattleSimulator.Brains;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
	public class UnearthSpellSettings : SpellSettings
	{
		public float GraveTime; // resurrect graves from the past X seconds.

		public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
		{
			return new UnearthSpell(parent, this, world);
		}
	}

	public class UnearthSpell : Spell<UnearthSpellSettings>
	{
		bool isExecuted;

		public UnearthSpell(BattleObject caster, UnearthSpellSettings settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
		{
		}

        public override void Tick()
        {
            if (!isExecuted)
            {
				isExecuted = true;
				OnExecute();
				Deactivate();
			}
        }

		private void OnExecute()
		{
			var graves = GameWorld.GetCreepsThatDiedSince(GameWorld.CurrentTime - Settings.GraveTime);
            foreach(var grave in graves)
            {
				var newUnit = GameWorld.SpawnUnit(grave.Settings, grave.Position, Owner, this);
				newUnit.SetBrain(new HoldGroundBrain());
			}
		}
	}
}