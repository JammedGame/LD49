using Game.Simulation;

namespace BattleSimulator.Spells
{
    public abstract class WildMagicSpell : Spell
    {
        public WildMagicSpell(SpellSettings settings, GameWorld gameWorld) : base(null, settings, gameWorld)
        {
        }
    }
}