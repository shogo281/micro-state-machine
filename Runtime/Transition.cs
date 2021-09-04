namespace MicroStateMachine
{
    /// <summary>
    /// ‘JˆÚ
    /// </summary>
    public class Transition : ITransition
    {
        private IState m_From = null;
        private IState m_To = null;
        private ICondition m_Condition = null;

        public Transition(IState from, IState to, ICondition condition)
        {
            Set(from, to, condition);
        }

        public IState From => m_From;

        public IState To => m_To;

        public ICondition Condition => m_Condition;

        public void Set(IState from, IState to, ICondition condition)
        {
            m_From = from;
            m_To = to;
            m_Condition = condition;
        }

        public void Dispose()
        {
            Set(null, null, null);
        }
    }
}