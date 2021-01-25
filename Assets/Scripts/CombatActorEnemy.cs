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
    public IntentDisplay intentDisplay;
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
        UpdateMoveDisplay(deck[0]);
    }

    public void UpdateMoveDisplay(CardData cardData)
    {
        intentDisplay.RecieveIntent(cardData.SelfEffects, cardData.Effects);
    }
    public void TakeTurn()
    {
        deck[0].SelfEffects.ForEach(x => healthEffects.RecieveEffect(x));
        deck[0].Effects.ForEach(x => combatController.Hero.healthEffects.RecieveEffect(x));

        discard.Add(deck[0]);
        deck.RemoveAt(0);
        if (deck.Count == 0)
        {
            deck.AddRange(discard);
            ShuffleDeck();
            discard.Clear();
        }
        UpdateMoveDisplay(deck[0]);
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
        combatController.CardUsed(this);
    }
}
