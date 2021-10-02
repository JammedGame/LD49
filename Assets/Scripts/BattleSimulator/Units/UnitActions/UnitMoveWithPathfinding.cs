using System;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game.Simulation
{
	public class UnitMoveWithPathfinding : UnitAction
	{
		public UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT)
		{
			// get target position using pathfinding
			var targetPosition = actionContext.Target.Position;
			var distanceToTarget = math.distance(unit.Position, targetPosition);

			// end movement action if already on the target - avoids floating point weirdos.
			if (distanceToTarget <= 0.001f)
			{
				unit.MoveToPosition(targetPosition);
				return UnitActionType.EndCurrentAction;
			}

			// get direction and update orientation towards target angle.
			var targetDirection = Pathfinding.CalculateTargetDirection(unit, actionContext.Target);
			var targetOrientation = MathUtil.ConvertDirectionToOrientation(targetDirection);
			unit.RotateTowardTarget(targetOrientation, dT);

			// move towards, but only if moving towards the goal, or if far enough to be able to ignore change in direction.
			var rotationDirection = MathUtil.ConvertOrientationToDirection(unit.Orientation);
			var directionMatch = Vector2.Dot(targetDirection, rotationDirection);

			// if too close, threshold should be very harsh (1)
			// if far away, we can afford to start moving even if direction to target is not perfectly aligned with the forward direction
			var secondsToTarget = distanceToTarget / unit.Speed;
			var directionMatchThreshold = Mathf.Clamp(1f - secondsToTarget, -1f, 1f - 0.02f);
			var speed01 = Mathf.Clamp01((directionMatch - directionMatchThreshold) / (1 - directionMatchThreshold));

			// move towards goal - end action if reached it
			var movementDelta = unit.Speed * speed01 * dT;
			if (movementDelta > distanceToTarget)
			{
				unit.MoveToPosition(targetPosition);
				return UnitActionType.EndCurrentAction;
			}
			else
			{
				var newPosition = unit.Position + movementDelta * rotationDirection;
				unit.MoveToPosition(newPosition);
				return UnitActionType.Movement;
			}
		}
	}
}