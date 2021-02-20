using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using System;

public class CombatController : StateMachine
{
    public GameObject TemplateCard;
    public BezierPath bezierPath;
    public GameObject TemplateEnemy;
    public List<EnemyData> enemyDatas = new List<EnemyData>();
    public TMP_Text lblEnergy;
    public List<Transform> trnsEnemyPositions;
    public Camera CombatCamera;
    public GameObject content;
    public Transform cardPanel;
    public Transform cardHoldPos;

    public CombatActorHero Hero; 

    public GameObject txtDeck;
    public GameObject txtDiscard;
    public int HandSize = 10;
    public int DrawCount = 5;
    public float offset = 50;
    public float handDistance = 150;
    public float handHeight = 75;
    public int energyTurn = 3;
    public float origoCardRot = 1000f;
    public float origoCardPos = 1000f;
    public float handDegreeBetweenCards = 10f;
    public CombatActorEnemy targetedEnemy;

    public AnimationCurve transitionCurve;
    public bool acceptSelections = true;
    public bool acceptActions = true;
    public Canvas canvas;
    private CombatActorEnemy _activeEnemy;


    KeyCode[] AlphaNumSelectCards = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    [HideInInspector]
    public int backingEnergy;
    [HideInInspector]
    public int cEnergy
    {
        get { return backingEnergy; }
        set { backingEnergy = value; lblEnergy.text = backingEnergy.ToString(); }
    }

    private List<CardData> DeckData;
    public List<CardCombatAnimated> Deck = new List<CardCombatAnimated>();
    public List<CardCombatAnimated> Hand = new List<CardCombatAnimated>();
    public List<CardCombatAnimated> Discard = new List<CardCombatAnimated>();
    public List<CardCombatAnimated> createdCards = new List<CardCombatAnimated>();

    public List<CombatActorEnemy> EnemiesInScene = new List<CombatActorEnemy>();
    public bool mouseInsidePanel = false;

    // we could remove enemies from list but having another list will 
    // make it easier to reference previous enemies for resurection etc
    public List<CombatActorEnemy> DeadEnemiesInScene = new List<CombatActorEnemy>();

    //[HideInInspector]
    public CardCombatAnimated _activeCard;
    [HideInInspector]
    public CombatActorEnemy ActiveEnemy
    {
        get
        {
            return _activeEnemy;
        }
        set
        {
            _activeEnemy = value;
            if (!(ActiveCard is null)) 
                ActiveCard.animator.SetBool("HasTarget",!(value is null));

        }
    }

    public CardCombatAnimated ActiveCard 
    {
        get
        {
            return _activeCard;
        }
        set
        {
            _activeCard = value;
            EnemiesInScene.ForEach(x => x.SetTarget(false));
            foreach (CardCombatAnimated card in Hand)
                if (card != value) card.MouseReact = (value is null);
        }
    }


    #region Plumbing, Setup, Start/End turn

    public void SetUpEncounter(List<EnemyData> enemyDatas = null)
    {

        DeckData = WorldSystem.instance.characterManager.playerCardsData;

        foreach(CardData cd in DeckData)
        {
            CardCombatAnimated card = CardCombatAnimated.CreateCardFromData(cd, this);
            Deck.Add(card);
        }

        ShuffleDeck();

        if (enemyDatas == null)
            enemyDatas = WorldSystem.instance.encounterManager.currentEncounter.encounterData.enemyData;

        for (int i = 0; i < enemyDatas.Count; i++)
        {
            GameObject EnemyObject = Instantiate(TemplateEnemy, trnsEnemyPositions[i].position, Quaternion.Euler(0, 0, 0), this.transform) as GameObject;
            CombatActorEnemy combatActorEnemy = EnemyObject.GetComponent<CombatActorEnemy>();
            combatActorEnemy.combatController = this;
            combatActorEnemy.ReadEnemyData(enemyDatas[i]);
            EnemiesInScene.Add(combatActorEnemy);
        }

        SetState(new EnterCombat(this));
    }

    public void StartTurn()
    {
        StartCoroutine(RulesSystem.instance.StartTurn());
    }

    public void EndTurn()
    {
        Hero.healthEffects.EffectsOnNewTurnBehavior();

        foreach (CardCombatAnimated card in Hand)
        {
            card.MouseReact = false;
            card.selectable = false;
        }

        StartCoroutine(DiscardAllCards());
    }

    private void KillEnemy(CombatActorEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        DeadEnemiesInScene.Add(enemy);
        EnemiesInScene.Remove(enemy);
    }

    //called from DEBUG
    public void WinCombat()
    {
        while (EnemiesInScene.Count > 0)
        {
            KillEnemy(EnemiesInScene[0]);
        }
        Debug.Log("Victory!");
        ResetCombat();
        WorldSystem.instance.uiManager.rewardScreen.GetCombatReward();
    }

    void ResetCombat()
    {
        DeadEnemiesInScene.Clear();
        Deck.Clear();
        Hand.Clear();
        Discard.Clear();
        EnemiesInScene.Clear();
        foreach (CardCombatAnimated card in createdCards)
        {
            DestroyImmediate(card);
        }
    }
    #endregion

    #region Positioning and Scale
    public Vector3 GetCardScale()
    {
        return Vector3.one;
    }

    public (Vector3 Position, Vector3 Angles) GetPositionInHand(CardCombatAnimated card)
    {
        return GetPositionInHand(Hand.IndexOf(card));
    }

    public (Vector3 Position,Vector3 Angles) GetPositionInHand(int CardNr)
    {
        // Positional Info
        float localoffset = (Hand.Count % 2 == 0) ? handDegreeBetweenCards/2 : 0;
        float degreeRad = Mathf.Deg2Rad * ((CardNr - (Hand.Count / 2)) * handDegreeBetweenCards + localoffset);
        Vector3 newPos = new Vector3(origoCardPos * Mathf.Sin(degreeRad), origoCardPos * Mathf.Cos(degreeRad) - origoCardPos + handHeight, 0);
        
        //Angling info
        Vector3 origo = new Vector3(0, -origoCardRot, 0);
        float angle = Vector3.Angle(newPos - origo, new Vector3(0, 1, 0))*(newPos.x > 0 ? -1 :1);
        Vector3 angles = new Vector3(0, 0, angle);

        return (newPos, angles);
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            if(!(Hand[i] == ActiveCard))
                Hand[i].transform.SetSiblingIndex(i);
    }


    public void RefreshHandPositions(CardCombatAnimated excludeCard = null)
    {
        foreach (CardCombatAnimated card in Hand)
            if (card != excludeCard)
                card.animator.SetBool("ReachedIdle", false);
    }
    #endregion

    #region Draw and Discard Cards, Deck Management

    public IEnumerator DrawCards(int CardsToDraw) { 
        for(int i = 0; i < CardsToDraw; i++)
        {
            if (Hand.Count == HandSize)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Hand is full yo");
                break;
            }

            if (Deck.Count == 0)
            {
                Deck.AddRange(Discard);
                Discard.Clear();
                ShuffleDeck();
            }

            if (Deck.Count == 0)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("no cards to draw broooow");
                break;
            }

            DrawSingleCard();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void DrawSingleCard()
    {
        CardCombatAnimated card = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(card);
        card.animator.SetTrigger("StartDraw");
        UpdateDeckTexts();
        RefreshHandPositions(card);
    }

    internal void CancelCardSelection()
    {
        ActiveCard = null;
        ResetSiblingIndexes();
    }


    private void ShuffleDeck()
    {
        for(int i = 0; i < Deck.Count; i++)
        {
            CardCombatAnimated temp = Deck[i];
            int index = UnityEngine.Random.Range(i, Deck.Count);
            Deck[i] = Deck[index];
            Deck[index] = temp;
        }
    }

    public IEnumerator DiscardAllCards()
    {
        while (Hand.Count > 0)
        {
            yield return StartCoroutine(DiscardCard(Hand[Hand.Count - 1]));
        }
    }

    public IEnumerator DiscardCard(CardCombatAnimated card)
    {
        Hand.Remove(card);
        Discard.Add(card);

        card.animator.SetTrigger("Discarded");
        RefreshHandPositions();
        yield return new WaitForSeconds(0.1f);
        UpdateDeckTexts();
    }

    public void UpdateDeckTexts()
    {
        txtDeck.GetComponent<Text>().text = "Deck:\n" + Deck.Count;
        txtDiscard.GetComponent<Text>().text = "Discard:\n" + Discard.Count;
    }

    #endregion

    #region CardUsage logic, user input
    void Update()
    {
        for (int i = 0; i < AlphaNumSelectCards.Length && i < Hand.Count; i++)
        {
            if (Input.GetKeyDown(AlphaNumSelectCards[i]) && WorldSystem.instance.worldState == WorldState.Combat)
            {
                if (ActiveCard == Hand[i])
                    ActiveCard.OnMouseRightClick(false);
                else
                    Hand[i].OnMouseClick();

                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && acceptSelections == true)
        {
            PlayerInputEndTurn();
        }
    }

    public void DetectCanvasClick()
    {
        if (ActiveCard != null)
        {
            if (Input.GetMouseButton(0))
                ActiveCard.OnMouseClick();
            else
                ActiveCard.OnMouseRightClick(false);
        }
    } 

    public void PlayerInputEndTurn()
    {
        // have to ahve a function for the ui button
        EndState();
    }

    public bool CardisSelectable(CardCombatAnimated card, bool silentCheck = true)
    {
        bool selectable = card.cardData.cost <= cEnergy && card.selectable;
        if (!silentCheck && card.cardData.cost > cEnergy)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough energy!");    
        }

        return selectable;
    }

    private bool MouseInsideArea()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardPanel.GetComponent<RectTransform>(), Input.mousePosition, WorldSystem.instance.cameraManager.mainCamera, out result);
        Debug.Log(result);
        Debug.Log(cardPanel.GetComponent<RectTransform>().rect.Contains(result));

        if (cardPanel.GetComponent<RectTransform>().rect.Contains(result))
        {
            Debug.Log("True");
            return true;
        }

        Debug.Log("False");
        return false;
    }

    public void CardUsed(CombatActorEnemy enemy = null)
    {
        if (!acceptActions)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Previous card is being resolved");
            return;
        }

        Debug.Log("Card used from Controller");
        targetedEnemy = enemy;
        if (ActiveCard is null)
            return;

        if ((ActiveCard.cardData.targetRequired && enemy is null) || MouseInsideArea())
        {
            ActiveCard.DeselectCard();
            return;
        }

        ActiveCard.animator.SetTrigger("MouseClicked");
        Hand.Remove(ActiveCard);


        RefreshHandPositions();

        cEnergy -= ActiveCard.cardData.cost;

        RulesSystem.instance.CarryOutCardSelf(ActiveCard.cardData, Hero);

        List<CombatActorEnemy> targetedEnemies = new List<CombatActorEnemy>();
        if (ActiveCard.cardData.targetRequired)
        {
            if (enemy != null)
                targetedEnemies.Add(enemy);
        }
        else
            targetedEnemies.AddRange(EnemiesInScene);


        foreach (CombatActorEnemy e in targetedEnemies)
        {
            RulesSystem.instance.CarryOutCard(ActiveCard.cardData, Hero, e);
            if (e.healthEffects.hitPoints <= 0)
                KillEnemy(e);
        }

        ActiveCard.cardData.activities.ForEach(x => StartCoroutine(CardActivitySystem.instance.StartByCardActivity(x)));

        ActiveCard = null;
    }

    #endregion , user input
}
