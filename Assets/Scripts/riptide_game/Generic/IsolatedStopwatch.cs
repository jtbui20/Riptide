public class IsolatedStopwatch
{
    private float elapsedTime;

    public bool isRunning { get; private set; }
    public bool isPaused { get; private set; }

    public event System.Action<float> OnTick;

    public void Start()
    {
        if (isRunning) return;
        Reset();
        isRunning = true;
        isPaused = false;
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
        isPaused = true;
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
            OnTick?.Invoke(elapsedTime);
        }
    }

    public float GetTimeElapsed()
    {
        if (!isRunning) return 0f;
        return elapsedTime;
    }
}