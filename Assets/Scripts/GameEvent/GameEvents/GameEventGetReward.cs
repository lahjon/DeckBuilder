using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class GameEventGetReward : GameEvent
{
    public override void TriggerGameEvent()
    { 
        var st1 = new Stopwatch();
        st1.Start();
        RewardType rewardType = gameEventStruct.parameter.ToEnum<RewardType>();
        st1.Stop();
        var st2 = new Stopwatch();
        st2.Start();
        //UnityEngine.Debug.Log("Time elapsed (ns): {0}", st1.Elapsed.TotalMilliseconds);

        world.rewardManager.CreateReward(rewardType, gameEventStruct.value);
    }
}