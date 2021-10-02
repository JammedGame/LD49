using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Game.Simulation
{
	public static class Pathfinding
	{
		/* settings */
		private const float ChangeDirectionPenalty = 0.2f;
		private const float AwayFromTargetPenalty = 0.5f;
		private const float GoodEnoughScore = 0.1f;
		private const float MaxObstacleDist = 1f;

		/* gameplay data */
		private static readonly Rotator Rotator = new Rotator();

		public static float2 CalculateTargetDirection(Unit unit, UnitTargetInfo targetInfo)
		{
			var board = unit.GameWorld.Board;
			var obstacles = unit.GameWorld.AllUnits;
			var direction = unit.GetDirection();
			var targetPosition = targetInfo.Position;
			var distToTarget = length(targetPosition - unit.Position);
			var raycastDistance = min(distToTarget, MaxObstacleDist);
			var initDir = normalizesafe(targetPosition - unit.Position, direction);

			// go through directions to find the best.
			var bestDir = initDir;
			var bestScore = 999f;

			// rotator determines how many rotations of the initial direction we are testing
			for (var j = 0; j < Rotator.Count; j++)
			{
				var dir = Rotator.GetRotation(initDir, j);
				var targetPos = unit.Position + dir * raycastDistance;

				// check if out of bounds
				if (targetPos.x < board.MinX || targetPos.x > board.MaxX ||
					targetPos.y < board.MinY || targetPos.y > board.MaxY)
						continue;

				// add cost for staying away from target / changing direction
				var cost = (1 - dot(dir, initDir)) * AwayFromTargetPenalty;
				cost += (1 - dot(dir, direction)) * ChangeDirectionPenalty;

				// add cost for obstacles
				foreach (var obstacle in obstacles)
				{
					if (obstacle == unit)
						continue;
					if (obstacle == targetInfo.TargetObject)
						continue;

					var distToObstacle = Math2D.GetDistanceToPoint(unit.Position, targetPos, obstacle.Position);
					if (distToObstacle < obstacle.Radius + unit.Radius)
					{
						var massRatio = obstacle.IsStatic ? 999f : 1f;
						cost += (1f - distToObstacle / (obstacle.Radius + unit.Radius)) *
							min(10, massRatio * massRatio);
					}
				}

				if (cost < bestScore)
				{
					bestScore = cost;
					bestDir = dir;
				}

				if (cost < GoodEnoughScore) break;
			}

			return bestDir;
		}
	}

	/// <summary>
	///     2D Rotation LUT.
	/// </summary>
	public class Rotator
	{
		public const int Count = 128;

		private readonly float2[] directions;

		public Rotator()
		{
			directions = new float2[Count];
			directions[0] = new float2(1f, 0f);
			directions[Count - 1] = new float2(-1f, 0f);

			for (var i = 1; i < Count - 1; i += 2)
			{
				var angle = i * PI / (Count - 2);
				directions[i] = new float2(cos(angle), sin(angle));
				directions[i + 1] = new float2(cos(-angle), sin(-angle));
			}
		}

		public float2 GetRotation(float2 dir, int i)
		{
			return new float2(dir.x * directions[i].x - dir.y * directions[i].y,
				dir.x * directions[i].y + dir.y * directions[i].x);
		}
	}
}