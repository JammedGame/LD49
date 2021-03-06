using Game.Simulation;
using JetBrains.Annotations;

namespace BattleSimulator.Spells
{
    public class EquippedSpell
    {
        public readonly SpellSettings SpellSettings;
        private float cooldownSecondsLeft = 0f;
        private Unit owner;

        public float CooldownProgress => 1f - cooldownSecondsLeft / SpellSettings.cooldownSeconds;
        public float CooldownSecondsLeft => cooldownSecondsLeft;
        public bool IsReady => cooldownSecondsLeft == 0;

        public EquippedSpell(SpellSettings spellSettings, Unit owner)
        {
            SpellSettings = spellSettings;
            this.owner = owner;
        }

        public void Tick()
        {
            cooldownSecondsLeft -= GameTick.TickDuration;
            if (cooldownSecondsLeft < 0)
            {
                cooldownSecondsLeft = 0;
            }
        }

        public void StartCooldown()
        {
            cooldownSecondsLeft = SpellSettings.cooldownSeconds;
        }
    }
}