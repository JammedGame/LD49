using System;
using Game.Simulation;
using Physics2D;
using Unity.Mathematics;
using UnityEngine;

namespace Game.View
{
	public class ProjectileView : BattleObjectView
	{
		public override void OnInitialized(BattleObject data)
		{
			if (!(data is Projectile))
			{
				Debug.LogError($"{this} only accepts data of type Projectile, not {data}", this);
				return;
			}
		}

		public override void SyncView(float dT)
		{
			var projectile = (Projectile)Data;

			// update transform
			SyncTransform(projectile);
		}

		public override void OnDeactivated()
		{
			base.OnDeactivated();
			SyncTransform((Projectile)Data);
		}

		private void SyncTransform(Projectile projectile)
		{
			transform.SetPositionAndRotation
			(
				projectile.Position,
				Quaternion.LookRotation(projectile.Velocity)
			);
		}
	}
}