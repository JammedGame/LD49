public class GameTick
{
	public const float TickDuration = 1f / 60f;
	public readonly int TickId;
	public float Time => TickId * TickDuration;

	public GameTick(int tickId)
	{
		TickId = tickId;
	}
}