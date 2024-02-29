using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityDataApi.Data
{
    public class RetryPolicyOptions
    {
        public int MaxRetryAttempts { get; set; }
        public int BackoffMultiplier { get; set; }
        public int MaxDelayMilliseconds { get; set; }
        public int InitialDelayMilliseconds { get; set; }
    }
}