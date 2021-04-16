using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DebugCommandBase : MonoBehaviour
{
    public string commandId;
    public string commandDescription;
    public string commandFormat;

    public DebugCommandBase(string id, string description, string format)
    {
        commandId = id;
        commandDescription = description;
        commandFormat = format;
    }
    
}

public class DebugCommand : DebugCommandBase
{
    Action command;
    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    Action<T1> command;
    public DebugCommand(string id, string description, string format, Action<T1> command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}