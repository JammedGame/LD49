using BattleSimulator.Spells;
using UnityEngine;

namespace Game.Simulation
{
    public class WildMagicController
    {
        private const float UnstableEnergyThreshold = 5f;
        private const float EnergyDecayPerSecond = 1f;
        private const float castChancePerEnergyOverflow = 0.02f;
        private const float castCooldownSeconds = 5f;

        public float Energy = 0f;

        public float EnergyProgress
        {
            get
            {
                float ret = Energy / UnstableEnergyThreshold;
                if (ret > 1)
                {
                    return 1;
                }
                return ret;
            }
        }
        private float castCooldownLeft = 0;
        public bool IsUnstable => Energy > UnstableEnergyThreshold;

        public void Tick(float dT)
        {
            Energy -= EnergyDecayPerSecond * dT;
            if (Energy < 0)
            {
                Energy = 0;
            }

            castCooldownLeft -= dT;
            if (castCooldownLeft < 0f)
            {
                castCooldownLeft = 0f;
            }

            if (IsUnstable)
            {
                float overflow = Energy - UnstableEnergyThreshold;
                float chance = castChancePerEnergyOverflow * overflow;
                if (Random.value < chance && castCooldownLeft == 0f)
                {
                    CastRandom();
                    castCooldownLeft = castCooldownSeconds;
                }
            }
        }

        public void OnSpellCast(SpellSettings spellSettings)
        {
            Energy += spellSettings.wildMagicEnergy;
        }

        private void CastRandom()
        {
            Debug.Log("A random magic spell is released!");
        }
    }
}