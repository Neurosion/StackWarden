using System;
using log4net;
using StackWarden.Core.Extensions;
using System.Collections.Generic;

namespace StackWarden.Monitoring.ResultHandling
{
    public class CompositeResultHandler : ResultHandler
    {
        private readonly IResultHandler[] _handlers;

        public bool ShouldStopProcessingOnSuccess { get; set; }

        public CompositeResultHandler(ILog log, params IResultHandler[] handlers)
            :base(log)
        {
            _handlers = handlers.ThrowIfNullOrEmpty(nameof(handlers));
        }

        protected override Result HandleAndGetResult(Result result)
        {
            var didSucceed = true;
            var childResults = new List<Result>();
            var addResult = new Action<Result>(r => childResults.Add(r));

            for (var i = 0; i < _handlers.Length || (ShouldStopProcessingOnSuccess && didSucceed); i++)
            {
                _handlers[i].Handled += addResult;
                didSucceed &= _handlers[i].Handle(result);
            }

            var newResult = GetNewResult();
            newResult.Chain = childResults.ToArray();

            return newResult;
        }
    }
}