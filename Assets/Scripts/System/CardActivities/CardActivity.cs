using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardActivity
{
    public static CardActivitySystem ActivitySystem;

    public abstract IEnumerator Execute(string input);
    public abstract string GetDescription(string input);

    public abstract string GetToolTip(string input);

}
