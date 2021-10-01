namespace Physics2D
{
	[System.Serializable]
	public class PhysicsSettings
	{
		public int Passes;
		public float VelocityDragConstant = 3;
		public float VelocityDragRelative = 0.98f;
		public float BounceAbsorbConstant = 1;
		public float BounceAbsorbRelative = 0.75f;
	}
}