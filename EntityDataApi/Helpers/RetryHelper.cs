using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityDataApi.Data;
using Microsoft.Extensions.Options;

namespace EntityDataApi.Helpers
{
    public class RetryHelper
    {
        private readonly RetryPolicyOptions _options;
        private readonly ILogger _logger;

        public RetryHelper(ILogger<RetryHelper> logger, IOptions<RetryPolicyOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<T> RetryWithBackoffAsync<T>(Func<Task<T>> action)
        {
            int retryCount = 0;
            while (retryCount < _options.MaxRetryAttempts)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Transient error occurred: {ex.Message}");
                    retryCount++;
                    if (retryCount < _options.MaxRetryAttempts)
                    {
                        TimeSpan delay = CalculateBackoffDelay(retryCount);
                        _logger.LogWarning($"Retrying attempt {retryCount} after {delay.TotalMilliseconds} milliseconds.");
                        await Task.Delay(delay);
                    }
                }
            }
            throw new InvalidOperationException("Max retry attempts reached.");
        }

        private TimeSpan CalculateBackoffDelay(int retryCount)
        {
            // Calculate exponential backoff delay with jitter(random value between 0&100)
            Random random = new Random();
            int jitter = random.Next(0, 100);
            double exponentialDelay = Math.Pow(_options.BackoffMultiplier, retryCount) * _options.InitialDelayMilliseconds;
            double delayMilliseconds = Math.Min(exponentialDelay + jitter, _options.MaxDelayMilliseconds);
            return TimeSpan.FromMilliseconds(delayMilliseconds);
        }
    }
}