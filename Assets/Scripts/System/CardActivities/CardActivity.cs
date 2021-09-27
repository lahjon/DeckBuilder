using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardActivity
{
    public static CardActivitySystem ActivitySystem;

    public abstract IEnumerator Execute(CardActivitySetting data);
    public abstract string GetDescription(CardActivitySetting data);
    public abstract string GetToolTip(CardActivitySetting data);

}
