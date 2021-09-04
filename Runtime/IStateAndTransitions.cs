using System;
using System.Collections.Generic;

namespace MicroStateMachine
{
    public class IStateAndTransitions : IDisposable
    {
        private IState m_From = null;
        private List<Transition> m_TransitionList = null;

        /// <summary>
        /// ひとつでもTrueになればtrueを返して、transitionにセットする。
        /// falseならtransitionはnull。
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public bool AnyTrue(out Transition transition)
        {
            transition = null;

            foreach (var tran in m_TransitionList)
            {
                if (tran.Condition.IsTrue == true)
                {
                    transition = tran;
                    return true;
                }
            }

            return false;
        }

        public IState Current => m_From;

        public IStateAndTransitions(IState from)
        {
            m_From = from;

            m_TransitionList = new List<Transition>();
        }

        public Transition CreateTransition(IState to, ICondition condition)
        {
            var transition = new Transition(m_From, to, condition);
            m_TransitionList.Add(transition);

            return transition;
        }

        public void Dispose()
        {
            foreach (var transition in m_TransitionList)
            {
                transition.Dispose();
            }

            m_TransitionList = null;
        }
    }
}
