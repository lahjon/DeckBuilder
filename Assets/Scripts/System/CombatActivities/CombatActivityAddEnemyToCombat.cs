using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CombatActivityAddEnemyToCombat : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        string[] ids = data.strParameter.Split(';');   

        foreach(string id in ids)
        {
            EnemyData enemyData = DatabaseSystem.instance.enemies.FirstOrDefault(e => e.enemyId == id.ToInt());
            if(enemyData == null)
            {
                Debug.LogWarning(string.Format("No enemy with id {0}", id));
                continue;
            }

            CombatSystem.instance.AddEnemy(enemyData);
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return string.Format("Add the an Enemy to the fight!");
    }

    public override string GetToolTip(CombatActivitySetting data) => string.Empty;
    
}
