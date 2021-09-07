using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace MicroStateMachine
{
    public class StateMachine : IDisposable
    {
        private bool isDisposed = false;
        private Dictionary<long, IStateAndTransitions> m_IStateAndTransitionsDictionary = null;
        private HashSet<IState> m_StateHashSet = null;

        private IStateAndTransitions Current { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateMachine()
        {
            isDisposed = false;
            m_IStateAndTransitionsDictionary = null;
            Current = null;
            m_StateHashSet = null;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            m_StateHashSet = new HashSet<IState>();
            m_IStateAndTransitionsDictionary = new Dictionary<long, IStateAndTransitions>();
            Current = null;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
#if UNITY_EDITOR
            if (isDisposed == true)
            {
                Debug.LogWarning("すでにDisposeが呼ばれている。");
                return;
            }
#endif

            if (Current == null)
            {
                return;
            }

            var anyTrue = Current.AnyTrue(out Transition transition);
            var currentState = Current.Current;

            if (anyTrue == false)
            {
                currentState.Begin();
                currentState.Update();
            }
            else
            {
                SetCurrentState(transition.To.ID);
            }
        }

        public void Dispose()
        {
            isDisposed = true;

            foreach (var iStaetAndTransitions in m_IStateAndTransitionsDictionary.Values)
            {
                iStaetAndTransitions.Dispose();
            }

            foreach (var state in m_StateHashSet)
            {
                state.Dispose();
            }
            m_IStateAndTransitionsDictionary = null;
        }

        public void AddState(IState state)
        {
            var id = state.ID;

            if (m_IStateAndTransitionsDictionary.ContainsKey(id) == false)
            {
                m_IStateAndTransitionsDictionary.Add(id, new IStateAndTransitions(state));
            }
        }

        public void AddState(IState from, ICondition condition, IState to)
        {
#if UNITY_EDITOR
            Debug.Assert(from != null);
            Debug.Assert(condition != null);
            Debug.Assert(to != null);
#endif
            AddState(from);
            AddState(to);

            var id = from.ID;
            var iStateAndTransitions = m_IStateAndTransitionsDictionary[id];
            iStateAndTransitions.CreateTransition(to, condition);

            if (Current == null)
            {
                Current = iStateAndTransitions;
            }

            m_StateHashSet.Add(from);
            m_StateHashSet.Add(to);
        }

        /// <summary>
        /// TStateでキャストできる最初の型のオブジェクトを取得する。
        /// なければnullを返す。
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public TState GetState<TState>() where TState : class, IState
        {
            return m_StateHashSet.FirstOrDefault(state => state is TState) as TState;
        }

        public void SetCurrentState(IState state)
        {
            SetCurrentState(state.ID);
        }

        public void SetCurrentState(long id)
        {
            Current.Current.End();
            Current = m_IStateAndTransitionsDictionary[id];
        }
    }
}