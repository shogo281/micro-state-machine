using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MicroStateMachine
{
    public class StateObject<TTrigger, TContext> : IDisposable where TContext : class
    {
        public bool IsBegan { get; private set; } = false;
        public bool IsStart { get; private set; } = false;

        protected StateMachine<TTrigger, TContext> StateMachine { get; private set; } = null;

        protected TContext Context { get; private set; } = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateMachine"></param>
        public StateObject(StateMachine<TTrigger, TContext> stateMachine, TContext context)
        {
            StateMachine = stateMachine;
            Context = context;
        }

        public virtual void OnBegin()
        {
            IsBegan = true;
        }

        public virtual void OnStart()
        {
            IsStart = true;
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnStop()
        {
            IsStart = false;
        }

        public virtual void OnEnd()
        {
            IsBegan = false;
        }

        public void Dispose()
        {
            if (IsStart == true)
            {
                OnStop();
            }

            if (IsBegan == true)
            {
                OnEnd();
            }

            StateMachine = null;
            Context = null;
        }
    }
}

