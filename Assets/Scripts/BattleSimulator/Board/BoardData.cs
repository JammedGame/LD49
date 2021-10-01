using System;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation.Board
{
	public partial class BoardData : ScriptableObject
	{
		[SerializeField] private int width;
		[SerializeField] private int height;

		int version; // if board gets changed, this will make other systems now they are out of sync with board state.

		public int Width => width;
		public int Height => height;
		public float MinX => 0;
		public float MaxX => width;
		public float MinY => 0;
		public float MaxY => height;
		public int Version => version;

		public static BoardData CreateNew(int width, int height)
		{
			var boardData = ScriptableObject.CreateInstance<BoardData>();
			boardData.width = width;
			boardData.height = height;
			return boardData;
		}


		/// <summary>
		/// Converts tile index into a 2d center of the tile.
		/// </summary>
		public float2 ConvertIndexToPosition(int index)
		{
			var x = index % width;
			var y = index / width;
			return new float2(x + 0.5f, y + 0.5f);
		}

		/// <summary>
		/// Converts tile coordinate into a 2d center of the tile.
		/// </summary>
		public float2 ConvertCoordinateToPosition(int x, int y)
		{
			return new float2(x + 0.5f, y + 0.5f);
		}

		/// <summary>
		/// Converts tile coordinate into a 2d center of the tile.
		/// </summary>
		public float2 ConvertCoordinateToPosition(int2 tileCoords)
		{
			return new float2(tileCoords.x + 0.5f, tileCoords.y + 0.5f);
		}

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPoint(float2 point)
		{
			if (point.x < MinX) point.x = MinX;
			if (point.x > MaxX) point.x = MaxX;
			if (point.y < MinY) point.y = MinY;
			if (point.y > MaxY) point.y = MaxY;
			return point;
		}

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPosition(float2 point, float size)
		{
			if (point.x < MinX + size) point.x = MinX + size;
			if (point.x > MaxX - size) point.x = MaxX - size;
			if (point.y < MinY + size) point.y = MinY + size;
			if (point.y > MaxY - size) point.y = MaxY - size;
			return point;
		}

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPoint(float2 point, float2 size)
		{
			if (point.x < MinX + size.x) point.x = MinX + size.x;
			if (point.x > MaxX - size.x) point.x = MaxX - size.x;
			if (point.y < MinY + size.y) point.y = MinY + size.y;
			if (point.y > MaxY - size.y) point.y = MaxY - size.y;
			return point;
		}

		/// <summary>
		/// Given a tile index, gets index of the neighbour tile, if such exists.
		/// </summary>
		private int GetNeighbourIndex(int currentTileIndex, Direction dir)
		{
			switch (dir)
			{
				case Direction.Dir4:
					return currentTileIndex % width > 0
						? (currentTileIndex - 1) : -1;

				case Direction.Dir8:
					return currentTileIndex / width < height - 1
						? currentTileIndex + width : -1;

				case Direction.Dir6:
					return currentTileIndex % width < width - 1
						? (currentTileIndex + 1) : -1;

				case Direction.Dir2:
					return currentTileIndex / width > 0
						? currentTileIndex - width : -1;

				default:
					throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// Given a tile coorindate, gets coordinate of the neighbour tile.
		/// </summary>
		private int2 GetNeighbourCoordinate(int2 coordinate, Direction dir)
		{
			switch (dir)
			{
				case Direction.Dir4:
					coordinate.x -= 1;
					return coordinate;

				case Direction.Dir6:
					coordinate.x += 1;
					return coordinate;

				case Direction.Dir8:
					coordinate.y += 1;
					return coordinate;

				case Direction.Dir2:
					coordinate.y -= 1;
					return coordinate;

				default:
					throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// Converts tile coordinates and direction into unique index of the edge related to this tile.
		/// </summary>
		public int ConvertTileCoordinatesToEdgeIndex(int x, int y, Direction dir)
		{
			switch (dir)
			{
				case Direction.Dir8:
					return (y + 1) * (width * 2 + 1) + x;
				case Direction.Dir4:
					return width + y * (width * 2 + 1) + x;
				case Direction.Dir6:
					return width + y * (width * 2 + 1) + (x + 1);
				case Direction.Dir2:
					return y * (width * 2 + 1) + x;

				default:
					throw new Exception();
			}
		}

		/// <summary>
		/// Converts vertex coordinates and direction into unique index of the edge connecting this vertex.
		/// </summary>
		public int ConvertVertexCoordinatesToEdgeIndex(int x, int y, Direction dir)
		{
			switch (dir)
			{
				case Direction.Dir6:
					return y * (width * 2 + 1) + x; // top edge of (x, y) tile
				case Direction.Dir4:
					return y * (width * 2 + 1) + x - 1; // top edge of (x - 1, y) tile
				case Direction.Dir8:
					return width + y * (width * 2 + 1) + x; // left edge of (x, y) tile
				case Direction.Dir2:
					return width + (y - 1) * (width * 2 + 1) + x; // left edge of (x, y - 1) tile

				default:
					throw new Exception();
			}
		}

		/// <summary>
		/// Converts vertex coordinates and direction into unique index of the edge connecting this vertex.
		/// </summary>
		public int ConvertVertexCoordinatesToEdgeIndex(int2 coords, Direction dir)
		{
			return ConvertVertexCoordinatesToEdgeIndex(coords.x, coords.y, dir);
		}

		/// <summary>
		/// Converts index of an edge into a pair of vertex indicies (connected by the same edge)
		/// </summary>
		public int2 ConvertEdgeIndexToVertexPair(int edgeIndex)
		{
			var y0 = edgeIndex / (width * 2 + 1);
			var x0 = edgeIndex - y0 * (width * 2 + 1);

			if (x0 < width) // horizontal edge
			{
				return new int2()
				{
					x = x0 + y0 * (width + 1),
					y = (x0 + 1) + y0 * (width + 1)
				};
			}
			else
			{
				return new int2() // vertical edge
				{
					x = (x0 - width) + y0 * (width + 1),
					y = (x0 - width) + (y0 + 1) * (width + 1)
				};
			}
		}

		/// <summary>
		/// Converts index of a tileboard vertex into its position.
		/// </summary>
		public float2 ConvertVertexCoordinatesToPosition(int x, int y) => new float2()
		{
			x = x,
			y = y
		};

		/// <summary>
		/// Converts cooridnates of a tileboard vertex into its index.
		/// </summary>
		public int ConvertVertexCoordinateToIndex(int x, int y)
		{
			return x + y * (width + 1);
		}

		/// <summary>
		/// Converts index of a tileboard vertex into its coordinates.
		/// </summary>
		public int2 ConvertVertexIndexToCoordinate(int index)
		{
			return new int2()
			{
				x = index % (width + 1),
				y = index / (width + 1)
			};
		}

		/// <summary>
		/// Converts index of a tileboard vertex into its position.
		/// </summary>
		public float2 ConvertVertexIndexToPosition(int index)
		{
			return new float2()
			{
				x = index % (width + 1),
				y = index / (width + 1)
			};
		}

		public Vector3 GetPosition3D(float2 pos)
		{
			return new Vector3(pos.x, 0, pos.y);
		}
	}
}