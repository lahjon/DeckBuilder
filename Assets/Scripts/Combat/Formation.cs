using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    public FormationType FormationType;
    public List<Transform> transforms = new List<Transform>();

    private Dictionary<CombatActorEnemy, int> enemyToSlot = new Dictionary<CombatActorEnemy, int>();
    private Dictionary<int, CombatActorEnemy> slotToEnemy = new Dictionary<int, CombatActorEnemy>();

    public bool HasSpace => enemyToSlot.Count != transforms.Count;

    public void RegisterOccupant(CombatActorEnemy enemy)
    {
        if (!HasSpace)
        {
            Debug.LogWarning("Tried to add enemy to full formation! Cancelled");
            return;
        }

        for(int i = 0; i < transforms.Count; i++)
        {
            if (!slotToEnemy.ContainsKey(i))
            {
                slotToEnemy[i] = enemy;
                enemyToSlot[enemy] = i;
                enemy.transform.position = transforms[i].position;
                break;
            }
        }
    }

    public void DeregisterOccupant(CombatActorEnemy enemy)
    {
        slotToEnemy.Remove(enemyToSlot[enemy]);
        enemyToSlot.Remove(enemy);
    }
}
