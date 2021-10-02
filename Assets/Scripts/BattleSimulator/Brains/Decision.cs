using System;
using Game.Simulation;

namespace BattleSimulator.Brains
{
	public class Decision
	{
		public readonly UnitAction Action;
		public readonly UnitTargetInfo Target;

		public Decision(UnitAction action, UnitTargetInfo target)
		{
			Action = action ?? throw new NullReferenceException();
			Target = target;
		}
	}
}