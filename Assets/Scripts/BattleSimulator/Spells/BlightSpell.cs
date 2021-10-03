
using System.Collections.Generic;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class BlightSpell : Spell
    {
        public int hopsRemaining;
        public float damagePerHop;
        public BlightSpellSettings settings;
        public BattleObject caster;
        public Unit currentTarget;
        public float2 coilPosition;

        private List<Unit> alreadyVisited = new List<Unit>();

        public BlightSpell(BattleObject caster, UnitTargetInfo targetInfo, SpellSettings settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
        {
            this.caster = caster;
            this.settings = settings as BlightSpellSettings;
            hopsRemaining = this.settings.maxHops;
            damagePerHop = this.settings.damagePerHop;
            coilPosition = caster.GetPosition2D();
            currentTarget = FindNextTarget();
            
            Debug.Log($"Blight spell initiated with target: {currentTarget}");
        }

        public override void Tick()
        {
            if (currentTarget == null || hopsRemaining <= 0)
            {
                Deactivate();
                return;
            }
            
            float2 delta = currentTarget.GetPosition2D() - coilPosition;
            float distance = math.distance(delta.x, delta.y);

            if (distance > 0.1f)
            {
                // keep moving, not close enough
                float2 direction = math.normalize(delta);
                coilPosition += direction * (settings.coilSpeed * GameTick.TickDuration);
                return;
            }
            
            // hit the unit and hop to the next
            alreadyVisited.Add(currentTarget);
            currentTarget.DealDamage(damagePerHop, caster);
            currentTarget = FindNextTarget();

            damagePerHop *= settings.damageDecay;
            hopsRemaining--;
        }

        private Unit FindNextTarget()
        {
            Unit closest = null;
            float closestDistance = float.MaxValue;

            foreach (Unit obj in GameWorld.AllUnits)
            {
                if (obj.Owner == caster.Owner || alreadyVisited.Contains(obj))
                {
                    continue;
                }

                float2 delta = obj.GetPosition2D() - coilPosition;
                float distance = math.distance(delta.x, delta.y);
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