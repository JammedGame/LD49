using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using System.Collections.Generic;
using UnityEngine.Events;
using Game.Simulation.Board;

namespace Game.Simulation
{
	public static class Pathfinding
	{
		/* settings */
		private const float AwayFromTargetPenalty = 0.5f;
		private const float GoodEnoughScore = 0.1f;
		private const float MaxObstacleDist = 3f;

		/* gameplay data */
		private static readonly Rotator Rotator = new Rotator();

		static readonly List<Unit> obstacles = new List<Unit>();
		static readonly List<Polygon> polygonObstacles = new List<Polygon>();

		public static float2 CalculateTargetDirection(Unit unit, UnitTargetInfo targetInfo)
		{
			var board = unit.GameWorld.Board;
			var direction = unit.GetDirection();
			var targetPosition = targetInfo.Position;
			var distToTarget = length(targetPosition - unit.Position);
			var raycastDistance = min(distToTarget, MaxObstacleDist);
			var initDir = normalizesafe(targetPosition - unit.Position, direction);

			// go through directions to find the best.
			var bestDir = initDir;
			var bestScore = 999f;

			obstacles.Clear();
			foreach(var candidate in unit.GameWorld.AllUnits)
			{
				if (candidate == unit)
					continue;
				if (candidate == targetInfo.TargetUnit)
					continue;
				if (unit.IsWithinRange(candidate, candidate.Radius + raycastDistance))
					obstacles.Add(candidate);
			}

			polygonObstacles.Clear();
			foreach (var candidate in unit.GameWorld.Board.Polygons)
			{
				if (candidate.Dist2Point(unit.Position) <= raycastDistance)
					polygonObstacles.Add(candidate);
			}

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

				// add cost for obstacles
				foreach (var obstacle in obstacles)
				{
					var distToObstacle = Math2D.GetDistanceToPoint(unit.Position, targetPos, obstacle.Position);
					if (distToObstacle < obstacle.Radius + unit.Radius)
					{
						var massRatio = obstacle.IsStatic ? 999f : 1f;
						cost += (1f - distToObstacle / (obstacle.Radius + unit.Radius)) *
							min(10, massRatio * massRatio);

						if (cost >= bestScore)
							continue;
					}
				}

				foreach(var polygon in polygonObstacles)
				{
					if (polygon.IsPointInside(targetPos))
					{
						cost = float.MaxValue;
						break;
					}
				}

				if (cost < bestScore)
				{
					bestScore = cost;
					bestDir = dir;
				}

				if (cost < GoodEnoughScore)
					break;
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