
using Game.Simulation;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class BlightSpell : Spell
    {
        public int hopsRemaining;
        public float damagePerHop;
        public BlightSpellSettings settings;
        public BattleObject caster;
        public BattleObject currentTarget;
        public Vector3 coilPosition;

        public BlightSpell(BattleObject caster, UnitTargetInfo targetInfo, SpellSettings settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
        {
            this.caster = caster;
            this.settings = settings as BlightSpellSettings;
            hopsRemaining = this.settings.maxHops;
            damagePerHop = this.settings.damagePerHop;
            coilPosition = caster.GetPosition3D();
            currentTarget = targetInfo.TargetObject;

            Debug.Log($"Blight spell initiated with target: {currentTarget}");
        }

        public override void Tick()
        {
            if (currentTarget == null)
            {
                Deactivate();
                return;
            }
            
            Vector3 delta = currentTarget.GetPosition3D() - coilPosition;
            float distance = delta.magnitude;

            if (distance < 0.1f)
            {
                currentTarget.DealDamage(damagePerHop, caster);
                currentTarget = FindNextTarget();
                
                if (currentTarget == null || hopsRemaining <= 1)
                {
                    Deactivate();
                    return;
                }

                damagePerHop *= settings.damageDecay;
                hopsRemaining--;
                delta = currentTarget.GetPosition3D() - coilPosition;
            }
            
            Vector3 direction = delta.normalized;
            coilPosition += direction * (settings.coilSpeed * GameTick.TickDuration);
        }

        private BattleObject FindNextTarget()
        {
            BattleObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (BattleObject obj in GameWorld.AllBattleObjects)
            {
                if (obj.Owner == caster.Owner)
                {
                    continue;
                }

                float distance = DistanceTo(obj);
                if (distance < settings.hopRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = obj;
                }
            }

            return closest;
        }

        public override void OnDeactivate()
        {
            Debug.Log("Blight spell deactivated");
        }
    }
}