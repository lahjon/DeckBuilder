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
    public List<CardCombat> Deck = new List<CardCombat>();
    public List<CardCombat> Hand = new List<CardCombat>();
    public List<CardCombat> Discard = new List<CardCombat>();
    public List<CardCombat> createdCards = new List<CardCombat>();

    public List<CombatActorEnemy> EnemiesInScene = new List<CombatActorEnemy>();
    private int amountOfEnemies;
    public bool mouseInsidePanel = false;

    float cardPanelwidth;

    // we could remove enemies from list but having another list will 
    // make it easier to reference previous enemies for resurection etc
    public List<CombatActorEnemy> DeadEnemiesInScene = new List<CombatActorEnemy>();

    //[HideInInspector]
    public CardCombat _activeCard;
    public CardCombat previousActiveCard;
    [HideInInspector]
    public CombatActorEnemy ActiveEnemy;
    public AnimationCurve transitionCurve;
    public bool acceptSelections = true;
    public bool acceptActions = true;
    private bool drawingCards = false;

    public Canvas canvas;

    public CardCombat ActiveCard 
    {
        get
        {
            return _activeCard;
        }
        set
        {
            _activeCard = value;
            EnemiesInScene.ForEach(x => x.SetTarget(false));
        }
    }


    #region Plumbing, Setup, Start/End turn

    public void SetUpEncounter(List<EnemyData> enemyDatas = null)
    {

        DeckData = WorldSystem.instance.characterManager.playerCardsData;
        DeckData.ForEach(x => Discard.Add(CreateCardFromData(x)));

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

        amountOfEnemies = EnemiesInScene.Count;

        SetState(new EnterCombat(this));
    }

    CardCombat CreateCardFromData(CardData cardData)
    {
        GameObject CardObject = Instantiate(TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(cardPanel, false);
        CardObject.transform.localScale = GetCardScale();
        CardObject.GetComponent<BezierFollow>().route = bezierPath.transform;
        CardCombat Card = CardObject.GetComponent<CardCombat>();
        Card.cardData = cardData;
        Card.cardPanel = cardPanel.GetComponent<RectTransform>();
        Card.combatController = this;
        Card.BindCardData();
        createdCards.Add(Card);
        HideCard(CardObject);
        return Card;
    }


    public void StartTurn()
    {
        StartCoroutine(RulesSystem.instance.StartTurn());
    }

    public void EndTurn()
    {
        Hero.healthEffects.EffectsOnNewTurnBehavior();

        foreach (CardCombat card in Hand)
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
        foreach (CardCombat card in createdCards)
        {
            DestroyImmediate(card);
        }
    }


    #endregion

    #region Positioning and Scale
    private void Awake()
    {
        cardPanelwidth = cardPanel.GetComponent<RectTransform>().rect.width;
    }
    private void HideCard(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(-10000, -10000, -10000);
        gameObject.SetActive(false);
    }


    public Vector3 GetCardScale()
    {
        return Vector3.one;
    }


    public (Vector3 Position, Vector3 Angles) GetPositionInHand(CardCombat card)
    {
        return GetPositionInHand(Hand.IndexOf(card));
    }

    public (Vector3 Position,Vector3 Angles) GetPositionInHand(int CardNr)
    {
        float localoffset = (Hand.Count % 2 == 0) ? handDegreeBetweenCards/2 : 0;
        Vector3 newPos = GetPositionInHandCircle((CardNr - (Hand.Count / 2)) *handDegreeBetweenCards + localoffset);
        Vector3 origo = new Vector3(0, -origoCardRot, 0);

        Vector3 direction = newPos - origo;
        float angle = Vector3.Angle(newPos - origo, new Vector3(0, 1, 0))*(newPos.x > 0 ? -1 :1);
        Vector3 angles = new Vector3(0, 0, angle);

        return (newPos, angles);
    }

    // 0 degree will yield top of curve.
    private Vector3 GetPositionInHandCircle(float degree)
    {
        degree = degree * Mathf.Deg2Rad;
        return new Vector3(origoCardRot*Mathf.Sin(degree),origoCardRot*Mathf.Cos(degree) - origoCardRot + handHeight,0); 
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.SetSiblingIndex(i);
    }


    public void RefreshHandPositionsAfterCardUsed(CardCombat excludeCard = null)
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            if (Hand[i] == excludeCard) continue;
            Hand[i].inTransition = false;
            Hand[i].MouseReact = true;
            (Vector3, Vector3) TransInfo = GetPositionInHand(i);
            Hand[i].StartLerpPosition(TransInfo.Item1, TransInfo.Item2);
        }
    }


    #endregion

    #region Draw and Discard Cards, Deck Management

    public IEnumerator DrawCards(int CardsToDraw) { 
        drawingCards = true;
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

            yield return StartCoroutine(DrawSingleCard());
        }
        drawingCards = false;
    }

    private IEnumerator DrawSingleCard()
    {
        CardCombat card = Deck[0];
        card.ResetCard();
        card.transform.localScale = Vector3.zero;
        card.transform.position = txtDeck.transform.position;
        card.gameObject.SetActive(true);
        card.MouseReact = false;
        Deck.RemoveAt(0);
        Hand.Add(card);
        UpdateDeckTexts();
        card.transform.SetAsLastSibling();
        RefreshHandPositionsAfterCardUsed(card);
        (Vector3, Vector3) TransInfo = GetPositionInHand(card);
        card.AnimateCardByCurve(TransInfo.Item1, TransInfo.Item2, Vector3.one, false, true);
        yield return new WaitForSeconds(0.2f);


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
            CardCombat temp = Deck[i];
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

    public IEnumerator DiscardCard(CardCombat card)
    {
        if (Hand.Contains(card)) Hand.Remove(card);
        Discard.Add(card);

        card.inTransition = true;
        card.AnimateCardByPathDiscard();
        RefreshHandPositionsAfterCardUsed();
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

    public bool CardisSelectable(CardCombat card, bool silentCheck = true)
    {
        bool selectable = card.cardData.cost <= cEnergy && card.selectable && !drawingCards;
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

        ActiveCard.AnimateCardUse();
        Hand.Remove(ActiveCard);
        ActiveCard = null;

        RefreshHandPositionsAfterCardUsed();

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
    }


    #endregion , user input


}
