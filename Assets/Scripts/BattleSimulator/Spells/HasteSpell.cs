using Game.Simulation;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class HasteSpell : Spell
    {
        private float secondsLeft;
        public HasteSpell(BattleObject caster, HasteSpellSettings settings, GameWorld gameWorld, OwnerId owner) : base(caster, settings, gameWorld)
        {
            Debug.Log($"{caster} casts Haste Spell!");
            secondsLeft = settings.EffectDurationSeconds;
            
        }

        public override void Tick()
        {
            secondsLeft -= GameTick.TickDuration;
            if (secondsLeft <= 0)
            {
                Deactivate();
            }
        }
    }
}