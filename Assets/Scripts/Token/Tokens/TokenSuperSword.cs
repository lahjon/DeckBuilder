using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSuperSword : Token
{
    public override void Init()
    {
        base.Init();
    }

    public override void AddActivity()
    {
        Debug.Log("Add Activity");
    }

    public override void RemoveActivity()
    {
        Debug.Log("Remove Activity");
    }
}
