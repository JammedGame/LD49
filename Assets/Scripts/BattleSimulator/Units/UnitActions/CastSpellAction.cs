using BattleSimulator.Spells;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    public class CastSpellAction : UnitAction
    {
        private EquippedSpell EquipedSpell;
        private float CastUpswing;
        private float CastSpeed => 1f / EquipedSpell.SpellSettings.castDurationSeconds;

        public CastSpellAction(EquippedSpell spellSettings, float upswing)
        {
            EquipedSpell = spellSettings;
            CastUpswing = upswing;
        }

        public UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT)
        {
            // if not currently in attack animation - break attack loop if needed.
            if (!actionContext.Started && ShouldBreakAttack(unit, actionContext.Target))
            {
                actionContext.ResetProgress();
                return unit.MovementAction.Tick(unit, ref actionContext, dT);
            }

            // update orientation in case we are not orientated properly
            unit.RotateTowardTarget(actionContext.Target.Position, dT);

            // update progress
            var oldProgress = actionContext.Progress;
            var newProgress = oldProgress + dT * CastSpeed;

            // land strike if reached the right frame
            if (!actionContext.Executed && newProgress >= CastUpswing)
            {
                unit.GameWorld.CastSpell(EquipedSpell.SpellSettings, actionContext.Target, unit);
				EquipedSpell.StartCooldown();
				actionContext.Executed = true;
			}

            // update progress
            actionContext.Progress = newProgress;
            actionContext.Started = true;

            if (newProgress >= 1f)
            {
                return UnitActionType.EndCurrentAction;
            }

            return UnitActionType.CastSpell;
        }

        private bool ShouldBreakAttack(Unit unit, UnitTargetInfo target)
        {
            if (EquipedSpell.SpellSettings.castRange == 0)
            {
                return false;
            }

            var distanceToTarget = math.distance(unit.Position, target.Position);
            if (distanceToTarget > EquipedSpell.SpellSettings.castRange)
            {
                return true;
            }

            return false;
        }
    }
}