using System;

namespace MicroStateMachine
{
    public interface ITransition : IDisposable
    {
        IState From { get; }
        IState To { get; }
        ICondition Condition { get; }
    }
}

