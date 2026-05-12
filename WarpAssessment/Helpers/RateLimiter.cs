namespace WarpAssessment.Helpers;

/// <summary>
/// Implements rate limiting to ensure requests don't exceed specified rate.
/// </summary>
public class RateLimiter
{
    private readonly int _requestsPerSecond;
    private readonly Queue<DateTime> _requestTimes;
    private readonly object _lockObject = new();

    public RateLimiter(int requestsPerSecond)
    {
        if (requestsPerSecond <= 0)
            throw new ArgumentException("Requests per second must be greater than 0", nameof(requestsPerSecond));
        
        _requestsPerSecond = requestsPerSecond;
        _requestTimes = new Queue<DateTime>(requestsPerSecond);
    }

    /// <summary>
    /// Waits if necessary to ensure rate limit is not exceeded, then records a new request.
    /// </summary>
    public async Task WaitIfNeededAsync()
    {
        lock (_lockObject)
        {
            var now = DateTime.UtcNow;
            var oneSecondAgo = now.AddSeconds(-1);

            // Remove requests older than 1 second
            while (_requestTimes.Count > 0 && _requestTimes.Peek() < oneSecondAgo)
            {
                _requestTimes.Dequeue();
            }

            // If we've hit the limit, calculate wait time
            if (_requestTimes.Count >= _requestsPerSecond)
            {
                var oldestRequest = _requestTimes.Peek();
                var waitTime = oldestRequest.AddSeconds(1) - now;
                
                if (waitTime > TimeSpan.Zero)
                {
                    // Wait synchronously for now - consider making this truly async
                    Thread.Sleep(waitTime);
                    now = DateTime.UtcNow;
                }
            }

            _requestTimes.Enqueue(now);
        }

        await Task.CompletedTask;
    }
}
