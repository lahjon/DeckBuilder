using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Enemy : MonoBehaviour
{
    public Material material;

    void Start()
    {
        // give each character a unique material
        material = Instantiate<Material>(material);
        Transform parent = transform.GetChild(0);
        SpriteRenderer spriteRenderer;

        for (int i = 0; i < parent.childCount; i++)
        {
            if(parent.GetChild(i).TryGetComponent<SpriteRenderer>(out spriteRenderer))
            {
                spriteRenderer.material = material;
            }
        }
    }
    public static Action EnemyKilledCallback(EnemyData enemy)
    {
        if(enemy == null) 
            return null;

        if (enemy.enemyId.Substring(0) == "2")
            return () => WorldSystem.instance.BossDefeated();
        
        return null;
    
    }
}
