using System;

namespace MicroStateMachine
{
    public class FuncCondition : Condition
    {
        private Func<bool> m_Func = null;

        public override bool IsTrue
        {
            get
            {
                if (m_Func == null)
                {
                    return false;
                }

                return m_Func();
            }
        }

        public FuncCondition()
        {
            SetFunc(null);
        }

        public FuncCondition(Func<bool> func)
        {
            SetFunc(func);
        }

        public void SetFunc(Func<bool> func)
        {
            m_Func = func;
        }

        public override void Dispose()
        {
            SetFunc(null);
            base.Dispose();
        }
    }
}
