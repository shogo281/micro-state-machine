using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;

namespace MicroStateMachine
{
    public interface IState : IDisposable
    {
        long ID { get; }
        void Begin();
        void Update();
        void End();
    }


    public class StateObject : IState
    {

        private static readonly ObjectIDGenerator OBJECT_ID_GENERATOR = new ObjectIDGenerator();
        public bool IsUpdate { get; private set; } = false;

        public long ID => OBJECT_ID_GENERATOR.GetId(this, out _);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateObject()
        {
        }

        protected virtual void OnBegin()
        {
        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            OnDispose();

            if (IsUpdate == true)
            {
                OnEnd();
            }
        }

        public void Begin()
        {
            if (IsUpdate == false)
            {
                OnBegin();
            }
            IsUpdate = true;
        }

        public void Update()
        {
            if (IsUpdate == true)
            {
                OnUpdate();
            }
        }

        public void End()
        {
            if (IsUpdate == true)
            {
                OnEnd();
            }
            IsUpdate = false;
        }
    }
}

