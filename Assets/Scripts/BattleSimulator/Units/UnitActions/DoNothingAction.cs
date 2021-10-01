namespace Game.Simulation
{
	public class DoNothingAction : UnitAction
	{
		public static readonly DoNothingAction Instance = new DoNothingAction();

		public UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT)
		{
			return UnitActionType.Idle;
		}
	}
}