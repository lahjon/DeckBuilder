using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Artifact : Item
{
    public override void BindData()
    {
        base.BindData();
    }
    public override void OnClick()
    {
        Debug.Log("Clicky");
    }
}
