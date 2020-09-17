using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateObject
{
    public bool IsBegan { get; private set; } = false;
    public bool IsStart { get; private set; } = false;

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
}

