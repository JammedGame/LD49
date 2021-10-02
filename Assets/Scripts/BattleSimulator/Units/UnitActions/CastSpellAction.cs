using BattleSimulator.Spells;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    public class CastSpellAction : UnitAction
    {
        private Spell Spell;
        private float CastUpswing;
        private float CastSpeed => 1f / Spell.Settings.castDurationSeconds;

        public CastSpellAction(Spell spell, float upswing)
        {
            Spell = spell;
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
            if (!actionContext.Executed && newProgress >= CastUpswing && oldProgress < CastUpswing)
            {
                Spell.Execute(unit, actionContext.Target);
                return UnitActionType.EndCurrentAction;
            }

            // update progress
            actionContext.Progress = newProgress;
            actionContext.Started = true;

            // make sure progress doesn't overflow
            if (newProgress > 1f)
            {
                actionContext.ResetProgress();
                actionContext.Progress = newProgress - 1f;
            }

            return UnitActionType.CastSpell;
        }
        
        private bool ShouldBreakAttack(Unit unit, UnitTargetInfo target)
        {
            if (Spell.Settings.castRange == 0)
            {
                return false;
            }
            
            var distanceToTarget = math.distance(unit.Position, target.Position);
            if (distanceToTarget > Spell.Settings.castRange)
            {
                return true;
            }
            
            return false;
        }
    }
}