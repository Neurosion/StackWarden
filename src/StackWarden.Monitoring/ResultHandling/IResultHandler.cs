using System;

namespace StackWarden.Monitoring.ResultHandling
{
    public interface IResultHandler
    {
        string Name { get; }
        event Action<Result> Handled;
        bool Handle(Result result);
    }
}