using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateMachine<TTrigger>
{
    private StateObject stateObject = null;
    private Dictionary<TTrigger, Type> stateTypeDictionary = null;
    private TTrigger requestState = default;
    private bool isRequestState = false;
    private bool canUpdate = true;

    /// <summary>
    /// 現在のState
    /// </summary>
    /// <value></value>
    public TTrigger CurrentState { get; private set; } = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public StateMachine()
    {
        CurrentState = default;
        stateObject = null;
        stateTypeDictionary = new Dictionary<TTrigger, Type>();
        canUpdate = false;
        isRequestState = false;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        canUpdate = true;
        CurrentState = default;
        requestState = default;
        stateObject = null;
        isRequestState = false;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
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
            CurrentState = requestState;
            stateObject.OnStop();
            stateObject.OnEnd();
            stateObject = Create(CurrentState);
            isRequestState = false;
        }
    }

    /// <summary>
    /// 終了
    /// </summary>
    public void Terminate()
    {
        canUpdate = false;
        stateObject = null;
    }

    /// <summary>
    /// Initialize前に呼ぶ
    /// StateとStateObjectのTypeを追加
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    public void AddStateType(TTrigger value, Type type)
    {
        stateTypeDictionary.Add(value, type);
    }

    /// <summary>
    /// Stateの削除
    /// </summary>
    /// <param name="value"></param>
    public void RemoveState(TTrigger value)
    {
        stateTypeDictionary.Remove(value);
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
    /// StateObjectの生成
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private StateObject Create(TTrigger value)
    {
        return Activator.CreateInstance(stateTypeDictionary[value]) as StateObject;
    }
}
