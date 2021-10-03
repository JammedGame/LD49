using System;
using System.Collections.Generic;
using BattleSimulator.Brains;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
	public class UnearthSpellSettings : SpellSettings
	{
		public float GraveTime; // resurrect graves from the past X seconds.
		public float MaxRadius = 100; // resurrect graves from the past X seconds.
		public float Duration = 5;

		public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
		{
			return new UnearthSpell(parent, this, world);
		}
	}

	public class UnearthSpell : Spell<UnearthSpellSettings>
	{
		bool isExecuted;
		float startExecutionTime;
		float2 castPosition;
		List<Creep> graves = new List<Creep>();
		public float CurrentRadius => Mathf.Lerp(0, Settings.MaxRadius, (GameWorld.CurrentTime - startExecutionTime) / Settings.Duration);

		public UnearthSpell(BattleObject caster, UnearthSpellSettings settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
		{
		}

        public override void Tick()
        {
            if (!isExecuted)
            {
				isExecuted = true;
				castPosition = Parent.GetPosition2D();
				startExecutionTime = GameWorld.CurrentTime;
				graves = GameWorld.GetCreepsThatDiedSince(GameWorld.CurrentTime - Settings.GraveTime);
			}

			float currentRadius = CurrentRadius;
			for (int i = 0; i < graves.Count; i++)
            {
				var grave = graves[i];
				var dist = math.distance(castPosition, grave.Position);
                if (dist <= currentRadius)
                {
					var newUnit = GameWorld.SpawnUnit(grave.Settings, grave.Position, Owner, this);
					newUnit.SetBrain(new AggroEverythingBrain());
					graves.RemoveAt(i--);
				}
			}
		}
	}
}