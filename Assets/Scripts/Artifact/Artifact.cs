using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Artifact : Item
{
    public Effect effect;
    public override void BindData(bool allData = true)
    {
        base.BindData();
    }
    public override void OnClick()
    {

    }
}
