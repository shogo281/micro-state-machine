using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroStateMachine
{
    /// <summary>
    /// 条件
    /// </summary>
    public class Condition : IDisposable, ICondition
    {

        public Condition()
        {
        }

        public virtual bool IsTrue
        {
            get
            {
                return true;
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
