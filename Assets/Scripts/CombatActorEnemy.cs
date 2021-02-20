using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CombatActorEnemy : CombatActor
{
    public CombatController combatController;
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

    public Canvas canvasEffects;
    public Canvas canvasIntent;
    public GameObject target;

    private void Start()
    {
        healthEffects.combatActor = this;
    }

    public void SetTarget(bool set = false)
    {
        if(set && !target.activeSelf)
        {
            target.SetActive(true);
        }
        else if(!set && target.activeSelf)
        {
            target.SetActive(false);
        }
    }

    public void ReadEnemyData(EnemyData inEnemyData = null)
    {
        if (!(inEnemyData is null))
            enemyData = inEnemyData;

        SetupCamera();

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

    public void SetupCamera()
    {
        canvasIntent.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvasIntent.planeDistance = WorldSystem.instance.uiManager.planeDistance;

        canvasEffects.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvasEffects.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void UpdateMoveDisplay(CardData cardData)
    {
        intentDisplay.RecieveIntent(cardData.Block, cardData.Damage, cardData.SelfEffects, cardData.Effects);   
    }
    public void TakeTurn()
    {
        RulesSystem.instance.CarryOutCardSelf(deck[0], this);
        RulesSystem.instance.CarryOutCard(deck[0], this, combatController.Hero);

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
        // Vector3 coordinates = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(AnchorMoveDisplay.transform.position);
        // CanvasMoveDisplay.transform.position = coordinates;
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
        if(combatController.ActiveCard != null  && WorldSystem.instance.combatManager.combatController.ActiveCard.targetRequired)
            SetTarget(true);


        if (combatController.ActiveEnemy is null) combatController.ActiveEnemy = this;
    }

    public void OnMouseExit()
    {
        SetTarget(false);
        if (combatController.ActiveEnemy == this) combatController.ActiveEnemy = null;
    }

    public void OnMouseDown()
    {
        combatController.CardUsed(this);
        Debug.Log("Press Enemy");
    }
}
