using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public abstract class BattleObject
	{
		/// <summary>
		/// Gameworld this object is part of.
		/// </summary>
		public readonly GameWorld GameWorld;

		/// <summary>
		/// Parent object - the one which created it.
		/// </summary>
		public readonly BattleObject Parent;

		/// <summary>
		/// Whether object is currently alive in the simulation.
		/// </summary>
		bool _isActive;

		/// <summary>
		/// Owner id of the object.
		/// </summary>
		OwnerId _owner;

		protected BattleObject(GameWorld gameWorld, OwnerId owner, BattleObject parent = null)
		{
			this.GameWorld = gameWorld;
			this.Parent = parent;
			_isActive = true;
			_owner = owner;
		}

		/// <summary>
		/// Whether object is currently alive in the simulation.
		/// </summary>
		public bool IsActive => _isActive;

		/// <summary>
		/// Owner id of the object.
		/// </summary>
		public OwnerId Owner => _owner;

		/// <summary>
		/// Deactivates object and marks it for removal.
		/// </summary>
		public void Deactivate()
		{
			if (_isActive)
			{
				GameWorld.DispatchViewEvent(this, ViewEventType.End);
				OnDeactivate();
				_isActive = false;
			}
		}

		/// <summary>
		/// Optional view path for objects that can be seen.
		/// </summary>
		public abstract string ViewPath { get; }

		/// <summary>
		/// What happens when this object receives damage?
		/// </summary>
		public virtual void DealDamage(float damage, BattleObject damageSource)
		{
		}

		/// <summary>
		/// Gets 2d position of the object.
		/// </summary>
		public abstract float2 GetPosition2D();

		/// <summary>
		/// Gets 3d position of the object.
		/// </summary>
		public abstract Vector3 GetPosition3D();

		/// <summary>
		/// Called when unit gets deactivated.
		/// </summary>
		public virtual void OnDeactivate() {}

		public float DistanceTo(BattleObject other)
		{
			return (other.GetPosition3D() - GetPosition3D()).magnitude;
		}
	}
}