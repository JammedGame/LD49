using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class UnitMoveWithDirectionAction : UnitAction
	{
		public static readonly UnitMoveWithDirectionAction Instance = new UnitMoveWithDirectionAction();

		public UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT)
		{
			// get direction and update orientation towards target angle.
			var targetDirection = actionContext.Target.Position;
			var targetOrientation = MathUtil.ConvertDirectionToOrientation(targetDirection);
			unit.RotateTowardTarget(targetOrientation, dT);

			// move towards, but only if moving towards the goal, or if far enough to be able to ignore change in direction.
			var rotationDirection = MathUtil.ConvertOrientationToDirection(unit.Orientation);
			var directionMatch = Vector2.Dot(targetDirection, rotationDirection);

			// if too close, threshold should be very harsh (1)
			// if far away, we can afford to start moving even if direction to target is not perfectly aligned with the forward direction
			var speed01 = math.clamp(math.length(targetDirection), 0, 1);

			// move towards goal - end action if reached it
			var movementDelta = unit.Settings.Speed * speed01 * dT;
			var newPosition = unit.Position + movementDelta * rotationDirection;
			unit.MoveToPosition(newPosition);
			return UnitActionType.Movement;
		}
	}
}