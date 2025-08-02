public class IsolatedStopwatch
{
    private float elapsedTime;

    private bool isRunning;

    private bool isPaused;

    public void Start()
    {
        if (isRunning) return;
        Reset();
        isRunning = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Pause()
    {
        if (!isRunning || isPaused) return;
        isPaused = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Reset()
    {
        Stop();
        elapsedTime = 0f;
        isPaused = false;
    }

    public void DoTick(float deltaTime)
    {
        if (isRunning && !isPaused)
        {
            elapsedTime += deltaTime;
        }
    }

    public float GetTimeElapsed()
    {
        if (!isRunning) return 0f;
        return elapsedTime;
    }
}