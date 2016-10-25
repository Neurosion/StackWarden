using System;
using log4net;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.ResultHandling
{
    public abstract class ResultHandler : IResultHandler
    {
        protected ILog Log { get; private set; }

        public string Name { get; set; }

        public ResultHandler(ILog log)
        {
            Log = log.ThrowIfNull(nameof(log));
        }

        public event Action<Result> Handled;

        protected abstract Result HandleAndGetResult(Result result);
        
        public bool Handle(Result result)
        {
            try
            {
                var newResult = HandleAndGetResult(result);
                Handled?.Invoke(newResult);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to handle result. Reason: {ex.Message}", ex);
            }

            return false;
        }

        protected Result GetNewResult()
        {
            return new Result
            {
                Source = new ResultSource
                {
                    Name = Name,
                    Type = GetType(),
                },
                TimeToLive = TimeSpan.Zero
            };
        }
    }
}