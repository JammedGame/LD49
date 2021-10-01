/// <summary>
/// Timeline which filters undeterministics time and undeterministics input commands into deterministics tick for the simulator.
/// </summary>
public class GameTimeline
{
	float undeterministicTime; // elapsed undeterministics time
	float deterministicTime; // provided deterministics time
	int nextTickId;

	public void ApplyUndeterministicTime(float dT)
	{
		undeterministicTime += dT;
	}

	/// <summary>
	/// Returns next deterministic tick if ready.
	/// </summary>
	public GameTick GetNextTick()
	{
		if (deterministicTime <= undeterministicTime)
		{
			var newTick = new GameTick(nextTickId++);
			deterministicTime = nextTickId * GameTick.TickDuration;
			return newTick;
		}

		return null;
	}
}

