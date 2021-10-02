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
	}
}