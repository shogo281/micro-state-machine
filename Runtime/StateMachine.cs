using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MicroStateMachine
{
    public class StateMachine<TTrigger, TContext> : IDisposable where TContext : class
    {
        /// <summary>
        /// Stateが切り替わったときのイベント
        /// </summary>
        public event Action OnChangeState = null;
        private readonly TContext context = null;
        private StateObject<TTrigger, TContext> stateObject = null;
        private Dictionary<TTrigger, StateObject<TTrigger, TContext>> triggerWithStateObjectDictionary = null;
        private TTrigger requestState = default;
        private bool isRequestState = false;
        private bool isDisposed = false;

        /// <summary>
        /// 現在のState
        /// </summary>
        /// <value></value>
        public TTrigger CurrentTrigger { get; private set; } = default;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateMachine()
        {
            CurrentTrigger = default;
            stateObject = null;
            triggerWithStateObjectDictionary = new Dictionary<TTrigger, StateObject<TTrigger, TContext>>();
            isRequestState = false;
            OnChangeState = null;
            isDisposed = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        public StateMachine(TContext context)
        {
            CurrentTrigger = default;
            stateObject = null;
            triggerWithStateObjectDictionary = new Dictionary<TTrigger, StateObject<TTrigger, TContext>>();
            isRequestState = false;
            OnChangeState = null;
            this.context = context;
            isDisposed = false;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            CurrentTrigger = default;
            requestState = default;
            stateObject = null;
            isRequestState = false;
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
            if (stateObject.IsBegan == false)
            {
                stateObject.OnBegin();
            }

            if (stateObject.IsStart == false)
            {
                stateObject.OnStart();
            }

            stateObject.OnUpdate();

            if (isRequestState == true)
            {
                CurrentTrigger = requestState;
                stateObject.OnStop();
                stateObject = GetStateObject(CurrentTrigger);
                isRequestState = false;

                if (OnChangeState != null)
                {
                    OnChangeState.Invoke();
                }
            }
        }

        public void Dispose()
        {
            foreach (var pair in triggerWithStateObjectDictionary)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                var instance = pair.Value;

                if (instance == null)
                {
                    continue;
                }

                instance.Dispose();
            }
            stateObject = null;
            OnChangeState = null;
            CurrentTrigger = default;
            isRequestState = false;
            triggerWithStateObjectDictionary = null;
            isDisposed = true;
        }

        /// <summary>
        /// トリガーとインスタンスを紐付ける。
        /// 呼び出し時にインスタンスを生成する
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="type"></param>
        public void AddTriggerAndCreateStateObjectInstance(TTrigger trigger, Type type)
        {
            AddTriggerAndInstance(trigger, CreateInstance(type));
        }

        /// <summary>
        /// トリガーとインスタンスを紐付ける。
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="stateObject"></param>
        public void AddTriggerAndInstance(TTrigger trigger, StateObject<TTrigger, TContext> stateObject)
        {
            if (triggerWithStateObjectDictionary.ContainsKey(trigger) == true)
            {
#if UNITY_EDITOR
                Debug.LogError("すでに追加しているトリガーです。");
#endif
                return;
            }

            triggerWithStateObjectDictionary.Add(trigger, stateObject);
        }

        /// <summary>
        /// トリガーとインスタンスを削除する
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool RemoveTriggerAndInstance(TTrigger trigger)
        {
            var isContains = triggerWithStateObjectDictionary.ContainsKey(trigger) == false;

            if (isContains == false)
            {
#if UNITY_EDITOR
                Debug.LogError("追加されていないトリガーを削除しようとしました。");
#endif
                return isContains;
            }

            var instance = triggerWithStateObjectDictionary[trigger];

            if (instance != null)
            {
                if (instance.IsStart == true)
                {
                    instance.OnStop();
                }

                if (instance.IsBegan == true)
                {
                    instance.OnEnd();
                }
            }

            return triggerWithStateObjectDictionary.Remove(trigger);
        }

        /// <summary>
        /// Stateの変更をリクエスト
        /// </summary>
        /// <param name="value"></param>
        public void RequestSetState(TTrigger value)
        {
            if (isRequestState == true)
            {
#if UNITY_ENGINE
            Debug.Warning("すでにStateの切り替えをリクエストしています。");
#endif
                return;
            }

            requestState = value;
        }

        /// <summary>
        /// StateObjectを生成する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private StateObject<TTrigger, TContext> CreateInstance(Type type)
        {
            var instance = Activator.CreateInstance(type, this, context) as StateObject<TTrigger, TContext>;

#if UNITY_EDITOR
            if (instance == null)
            {
                Debug.LogError(type.ToString() + "は無効。");
            }
#endif
            return instance;
        }

        private StateObject<TTrigger, TContext> GetStateObject(TTrigger trigger)
        {
#if UNITY_EDITOR
            if (triggerWithStateObjectDictionary.ContainsKey(trigger) == false)
            {
                Debug.LogError("追加していないトリガー。");
            }
#endif

            var instance = triggerWithStateObjectDictionary[trigger];

#if UNITY_EDITOR
            if (instance == null)
            {
                Debug.LogError(trigger.ToString() + "に紐付いているインスタンスはnullです。");
            }
#endif
            return instance;
        }
    }
}