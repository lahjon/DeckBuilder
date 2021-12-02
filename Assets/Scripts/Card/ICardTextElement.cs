using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardTextElement
{
    public string GetElementText();

    public string GetElementToolTip();

}

public interface ICardUpgradableComponent
{
    public bool CanAbsorb<T>(T modifier);

    public void AbsorbModifier<T>(T modifier);

    public void RegisterModified(ModifierType type);
}

public interface ICardUpgradingData
{

}
