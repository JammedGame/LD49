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
			var building = (Building)Data;

			// update transform
			transform.position = building.GetPosition3D();
		}

		void OnDrawGizmos()
		{
		}
	}
}