using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventUnlockCharacter : GameEvent
{
    public override void TriggerGameEvent()
    { 
        world.characterManager.UnlockCharacter(gameEventStruct.parameter.ToEnum<CharacterClassType>());
    }
}