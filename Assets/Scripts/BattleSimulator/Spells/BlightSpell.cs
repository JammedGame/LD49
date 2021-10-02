using Game.Simulation;

namespace BattleSimulator.Spells
{
    public class BlightSpell : Spell
    {
        private int hopsRemaining;
        private float damagePerHop;
        private BlightSpellSettings settings;
        private BattleObject currentTarget;
        public BlightSpell(BattleObject caster, UnitTargetInfo targetInfo, SpellSettings settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
        {
            this.settings = settings as BlightSpellSettings;
            hopsRemaining = this.settings.maxHops;
            damagePerHop = this.settings.damagePerHop;
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}