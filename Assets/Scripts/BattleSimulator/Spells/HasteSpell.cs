using System;
using Game.Simulation;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class HasteSpell : Spell
    {
        private float secondsLeft;
        private Unit caster;
        private float modifier;
        public HasteSpell(BattleObject caster, HasteSpellSettings settings, GameWorld gameWorld, OwnerId owner) : base(caster, settings, gameWorld)
        {
            if (!(caster is Unit))
            {
                throw new Exception("Haste spell can only be cast on Units!");
            }
            
            this.caster = (Unit) caster;
            modifier = settings.SpeedModifier - 1;
            secondsLeft = settings.EffectDurationSeconds;
        }

        public override void Tick()
        {
            secondsLeft -= GameTick.TickDuration;
            if (secondsLeft <= 0)
            {
                Deactivate();
                return;
            }
            
            caster.Speed.IncPercent(modifier);
        }
    }
}