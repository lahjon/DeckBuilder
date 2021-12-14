using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventUnlockProfession : GameEvent
{
    public override void TriggerGameEvent()
    { 
        WorldSystem.instance.characterManager.UnlockProfession(gameEventStruct.parameter.ToEnum<ProfessionType>(), gameEventStruct.value.ToBool());
    }
}