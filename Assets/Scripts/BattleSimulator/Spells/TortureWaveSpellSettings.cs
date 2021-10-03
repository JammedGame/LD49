using Game.Simulation;

namespace BattleSimulator.Spells
{
    public class TortureWaveSpellSettings : SpellSettings
    {
        public float coneLength;
        public float coneAngle;
        public float coneTravelSeconds;
        public float damage;
        public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
        {
            return new TortureWaveSpell(parent, this, world);
        }
    }
}