using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CombatActorEnemy : MonoBehaviour
{
    public CombatController combatController;
    public HealthEffects healthEffects;
    public TMP_Text txtMoveDisplay;
    public EnemyData enemyData;
    public GameObject CanvasMoveDisplay;
    public string enemyName;

    [HideInInspector]
    public List<CardData> deck;
    public List<CardData> discard;
    public CardData nextCard;
    public GameObject AnchorMoveDisplay;


    public void ReadEnemyData(EnemyData inEnemyData = null)
    {
        if (!(inEnemyData is null))
            enemyData = inEnemyData;

        deck.AddRange(enemyData.deck);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.artwork;
        enemyName = enemyData.enemyName;
        healthEffects.maxHitPoints = enemyData.StartingHP;
        healthEffects.hitPoints = enemyData.StartingHP;
        healthEffects.RemoveAllBlock();
        ShuffleDeck();
        SetUIpositions();
        UpdateMoveDisplay();
    }

    public void UpdateMoveDisplay()
    {
        if(deck.Count == 0)
        {
            deck.AddRange(discard);
            discard.Clear();
        }

        CardEffect damagePart = deck[0].Effects.Where(x => x.Type == EffectType.Damage).FirstOrDefault();
        if (!(damagePart is null))
            txtMoveDisplay.text = damagePart.Value.ToString() + (damagePart.Times == 1 ? "" : "x" + damagePart.Times.ToString());
        else
            txtMoveDisplay.text = "some effect";
    }
    public void TakeTurn()
    {
        CardEffect damagePart = deck[0].Effects.Where(x => x.Type == EffectType.Damage).FirstOrDefault();
        if (!(damagePart is null))
            combatController.Hero.GetComponentInChildren<HealthEffects>().TakeDamage(damagePart.Value * damagePart.Times);

        discard.Add(deck[0]);
        deck.RemoveAt(0);
        UpdateMoveDisplay();
    }



    public void SetUIpositions()
    {
        Vector3 coordinates = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(AnchorMoveDisplay.transform.position);
        CanvasMoveDisplay.transform.position = coordinates;
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int index = Random.Range(i, deck.Count);
            deck[i] = deck[index];
            deck[index] = temp;
        }
    }



    public void TakeDamage(int x)
    {
        healthEffects.TakeDamage(x);
    }

    public void OnMouseOver()
    {
        if (combatController.ActiveEnemy is null) combatController.ActiveEnemy = this;
    }

    public void OnMouseExit()
    {
        if (combatController.ActiveEnemy == this) combatController.ActiveEnemy = null;
    }

    public void OnMouseDown()
    {
        combatController.EnemyClicked(this);
    }
}
