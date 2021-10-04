using System.Collections.Generic;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class TortureWaveSpell : Spell
    {
        public TortureWaveSpellSettings settings;
        private float2 coneDirection;
        public float coneLength => growProgress * settings.coneLength;
        public Vector3 coneDirection3D => new Vector3(coneDirection.x, 0, coneDirection.y);
        public Vector3 coneCenterEnd3D => GetPosition3D() + coneDirection3D * coneLength;
        public Vector3 coneLeftEnd3D => GetPosition3D() + Quaternion.AngleAxis(-settings.coneAngle / 2f, Vector3.up) * coneDirection3D * coneLength;
        public Vector3 coneRightEnd3D => GetPosition3D() + Quaternion.AngleAxis(settings.coneAngle / 2f, Vector3.up) * coneDirection3D * coneLength;
        private float2 coneOrigin;
        private float growSpeed;
        public float growProgress;
        private int state;
        private HashSet<Unit> alreadyDamaged = new HashSet<Unit>();
        private Unit caster;
        private float holdSecondsLeft;
        
        // helpers
        private float minCos;
        public TortureWaveSpell(BattleObject caster, SpellSettings _settings, GameWorld gameWorld, float2 positionClicked) : base(caster, _settings, gameWorld)
        {
            this.caster = caster as Unit;
            settings = _settings as TortureWaveSpellSettings;
            coneOrigin = caster.GetPosition2D();
            coneDirection = math.normalize(positionClicked - caster.GetPosition2D());
            growSpeed = 1f / settings.coneGrowSeconds;
            growProgress = 0;
            state = 0;
            holdSecondsLeft = settings.holdSeconds;

            float degrees = settings.coneAngle / 2f;
            float radians = math.radians(degrees);
            minCos = math.cos(radians);
        }

        public override float2 GetPosition2D()
        {
            return coneOrigin;
        }

        public override Vector3 GetPosition3D()
        {
            return new Vector3(coneOrigin.x, 0f, coneOrigin.y);
        }

        public override void Tick()
        {
            base.Tick();

            switch (state)
            {
                case 0:
                    Grow();
                    break;
                case 1:
                    Hold();
                    break;
                case 2:
                    Shrink();
                    break;
                default:
                    Deactivate();
                    break;
            }
        }

        private void Grow()
        {
            growProgress += growSpeed * GameTick.TickDuration;
            DamageUnitsInCone();

            if (growProgress >= 1f)
            {
                state++;
            }
        }

        private void Hold()
        {
            holdSecondsLeft -= GameTick.TickDuration;
            DamageUnitsInCone();
            if (holdSecondsLeft <= 0f)
            {
                state++;
            }
        }

        private void Shrink()
        {
            growProgress -= GameTick.TickDuration * growSpeed;
            if (growProgress <= 0f)
            {
                state++;
            }
        }

        private void DamageUnitsInCone()
        {
            foreach (Unit unit in GameWorld.AllUnits)
            {
                if (unit.Owner == caster.Owner || alreadyDamaged.Contains(unit))
                {
                    // Friendly unit or already damaged
                    continue;
                }

                float2 directionToUnit = math.normalize(unit.GetPosition2D() - coneOrigin);
                float distance = math.distance(unit.GetPosition2D(), coneOrigin);
                float cos = math.dot(directionToUnit, coneDirection);
                
                if (cos < minCos || distance > coneLength)
                {
                    // Not in cone
                    continue;
                }

                unit.DealDamage(settings.damage, caster);
                alreadyDamaged.Add(unit);
            }
        }
    }
}