using System;
using System.Collections;
using System.Collections.Generic;
using Game.Simulation;
using Physics2D;
using UnityEngine;

namespace Game.View
{
	public class BuildingView : BattleObjectView
	{
		HealthBar healthBar;

		public override void OnInitialized(BattleObject data)
		{
			if (!(data is Building))
			{
				Debug.LogError($"{this} only accepts data of type Unit, not {data}", this);
				return;
			}
		}

		public override void SyncView(float dT)
		{
			base.SyncView(dT);
			transform.localPosition = Data.GetPosition3D();
			ViewController.HealthBarController.SyncHealthbar(this, ref healthBar);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			if (healthBar != null)
			{
				ViewController.HealthBarController.Dispose(healthBar);
				healthBar = null;
			}
		}

		public override void OnDispose()
		{
			base.OnDispose();
			if (healthBar != null)
			{
				ViewController.HealthBarController.Dispose(healthBar);
				healthBar = null;
			}
		}
	}
}