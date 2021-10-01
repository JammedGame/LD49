namespace Game.Simulation
{
	public interface UnitAction
	{
		/// <summary>
		/// Ticks unit action context. Executes various actions.
		/// Returns action type that unit is currently in (determines animation to be played and API)
		/// </summary>
		UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT);
	}

	public enum UnitActionType
	{
		Idle = 0,
		Movement = 1,
		Attack = 2,
		Special = 3,
		Death = 4,
		EndCurrentAction = -1
	}

	public static class UnitActionTypeExtensions
	{
		public static bool IsAnimationControlledBySimulation(this UnitActionType actionType)
		{
			return actionType == UnitActionType.Attack;
		}
	}
}