using Game.Simulation;
using Unity.Mathematics;

namespace BattleSimulator.Spells
{
    public class ImpaleSpell : Spell
    {
        public ImpaleSpellSettings settings;
        public float2 aoeCenter;
        public float protrudeSpeed;
        public float protrudeProgress;
        public bool executed = false;
        public Unit caster;
        private float protrudeHoldSecondsLeft;
        private int state = 0;
        public ImpaleSpell(BattleObject caster, float2 position, SpellSettings _settings, GameWorld gameWorld) : base(caster, _settings, gameWorld)
        {
            settings = _settings as ImpaleSpellSettings;
            aoeCenter = position;
            protrudeSpeed = 1f / settings.protrudeSeconds;
            protrudeProgress = 0f;
            this.caster = caster as Unit;
            protrudeHoldSecondsLeft = settings.protrudeHoldSeconds;
        }

        public override void Tick()
        {
            base.Tick();

            switch (state)
            {
                case 0:
                    TickProtrude();
                    break;
                case 1:
                    TickHold();
                    break;
                case 2:
                    TickRetract();
                    break;
                default:
                    Deactivate();
                    break;
            }
        }

        private void TickProtrude()
        {
            protrudeProgress += GameTick.TickDuration * protrudeSpeed;
            if (!executed && protrudeProgress > settings.spikeUpswing)
            {
                DealAoeDamage();
                executed = true;
            }

            if (protrudeProgress > 0.99f)
            {
                protrudeProgress = 1f;
                state++;
            }
        }

        private void TickHold()
        {
            protrudeHoldSecondsLeft -= GameTick.TickDuration;
            if (protrudeHoldSecondsLeft < 0.01f)
            {
                state++;
            }
        }

        private void TickRetract()
        {
            protrudeProgress -= protrudeSpeed * GameTick.TickDuration;
            if (protrudeProgress < 0.01f)
            {
                state++;
            }
        }

        private void DealAoeDamage()
        {
            foreach (Unit unit in GameWorld.AllUnits)
            {
                if (unit.Owner == caster.Owner)
                {
                    continue;
                }

                float distance = math.distance(aoeCenter, unit.GetPosition2D());
                if (distance <= settings.aoeRadius)
                {
                    unit.DealDamage(settings.damage, caster);
                }
            }
        }
    }
}