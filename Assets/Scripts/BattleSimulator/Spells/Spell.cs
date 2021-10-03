using System.Threading.Tasks;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public abstract class Spell : BattleObject
    {
        public readonly SpellSettings Settings;

        protected Spell(BattleObject caster, SpellSettings settings, GameWorld gameWorld) : base(gameWorld, caster.Owner, caster)
        {
            Settings = settings;
        }

        public override string ViewPath => $"View/SpellViews/{Settings.name}View";

        public bool IsSingleTarget => Settings.isSingleTarget;

        public override float2 GetPosition2D()
        {
            return float2.zero;
        }

        public override Vector3 GetPosition3D()
        {
            return Vector3.zero;
        }

		public override Vector3 GetCenterPosition3D()
		{
			return GetPosition3D();
		}

		public virtual void Tick()
        {

        }

        public bool ShouldCreepBeIncludedInTargets(Creep creep)
        {
            return creep.IsValidAttackTarget && creep.Owner != Owner;
        }
    }

    public abstract class Spell<T> : Spell where T : SpellSettings
    {
		protected Spell(BattleObject caster, T settings, GameWorld gameWorld) : base(caster, settings, gameWorld)
		{
		}

		public new T Settings => (T)base.Settings;
	}
}