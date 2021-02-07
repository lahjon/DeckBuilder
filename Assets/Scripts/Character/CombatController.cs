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
    public float handHeight = -75;
    public int energyTurn = 3;
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
    public List<GameObject> Deck = new List<GameObject>();
    public List<GameObject> Hand = new List<GameObject>();
    public List<GameObject> Discard = new List<GameObject>();
    public List<GameObject> createdCards = new List<GameObject>();
    public List<GameObject> discardCardEndTurn = new List<GameObject>();
    public List<GameObject> discardCardPlayerTurn = new List<GameObject>();

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
    public bool acceptInput = true;
    private bool drawingCards = false;

    public Canvas canvas;
    private void Awake()
    {
        cardPanelwidth = cardPanel.GetComponent<RectTransform>().rect.width;
    }

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

    void Update()
    {
        for(int i = 0; i < AlphaNumSelectCards.Length && i < Hand.Count; i++)
        {
            if (Input.GetKeyDown(AlphaNumSelectCards[i]) && WorldSystem.instance.worldState == WorldState.Combat)
            {
                if(ActiveCard && Hand[i].activeSelf && Hand[i].GetComponent<CardCombat>().inTransition == false)
                {
                    if(ActiveCard != Hand[i].GetComponent<CardCombat>())
                    {
                        ActiveCard.OnMouseRightClick(false);
                        Hand[i].GetComponent<CardCombat>().CardAction();
                    }
                    else
                    {
                        ActiveCard.OnMouseRightClick(false);
                    }
                }
                else
                {
                    if(Hand[i].GetComponent<CardCombat>().inTransition == false && Hand[i].activeSelf)
                    {
                        Hand[i].GetComponent<CardCombat>().CardAction();
                    }
                }
                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && acceptInput == true && discardCardEndTurn.Count == 0)
        {
            PlayerInputEndTurn();
        }
        // if(ActiveCard != null)
        // {
        //     MouseInsideArea();
        // }
    }

    public void DetectCanvasClick()
    {
        if(ActiveCard != null && ActiveCard.selected)
        {

            ActiveCard.selected = false;
            ActiveCard = null;
        }

    }

    public void PlayerInputEndTurn()
    {
        // have to ahve a function for the ui button
        EndState();
    }

    public void SetUpEncounter(List<EnemyData> enemyDatas = null)
    {

        DeckData = WorldSystem.instance.characterManager.playerCardsData;
        DeckData.ForEach(x => Discard.Add(CreateCardFromData(x)));

        if(enemyDatas == null)
            enemyDatas = WorldSystem.instance.encounterManager.currentEncounter.encounterData.enemyData;

        for(int i = 0; i < enemyDatas.Count; i++)
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

    public Vector3 GetCardScale()
    {
        return new Vector3(0.9f, 0.9f, 0.9f);
    }
    public Vector3 GetPositionInHand(int CardNr)
    {
        float midPoint = cardPanelwidth / 2;
        float localoffset = (Hand.Count % 2 == 0) ? offset : 0;
        return new Vector3(midPoint + (CardNr - Hand.Count / 2) * handDistance + localoffset, handHeight, 0);
    }

    public int GetCardNumberInHand(Card aCard)
    {
        return Hand.IndexOf(aCard.gameObject);
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.SetSiblingIndex(i);
    }


    private void DrawCards(int x)
    {
        for(int i = 0; i < x; i++)
        {
            if(Deck.Count == 0)
            {
                Deck.AddRange(Discard);
                Discard.Clear();
                ShuffleDeck();
            }

            if (Deck.Count != 0)
            {
                Hand.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }

        DisplayHand();
    }

    private void ShuffleDeck()
    {
        for(int i = 0; i < Deck.Count; i++)
        {
            GameObject temp = Deck[i];
            int index = UnityEngine.Random.Range(i, Deck.Count);
            Deck[i] = Deck[index];
            Deck[index] = temp;
        }
    }

    public bool CardisSelectable(Card card)
    {
        if (card.cardData.cost > cEnergy && !card.GetComponent<CardCombat>().selected && !drawingCards)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough energy!");    
        }
        Debug.Log(ActiveCard);
        return ActiveCard is null && card.cardData.cost <= cEnergy;
    }
    GameObject CreateCardFromData(CardData cardData)
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
        createdCards.Add(CardObject);
        HideCard(CardObject);
        return CardObject;
    }

    internal void CancelCardSelection(GameObject gameObject)
    {
        previousActiveCard = ActiveCard;
        int i = Hand.IndexOf(gameObject);
        gameObject.GetComponent<CardCombat>().AnimateCardByCurve(GetPositionInHand(i));
        ActiveCard = null;

        ResetSiblingIndexes();
    }

    public void EndTurn()
    {
        Hero.healthEffects.EffectsStartTurn();
        while (Hand.Count > 0)
        {
            GameObject card = Hand[0];
            discardCardEndTurn.Add(card);
            DiscardCard(card);
            DiscardCardToPile(card);
        }

        StartCoroutine(WaitForAnimationsDiscardEndTurn());
    }

    public void StartTurn()
    {
        // ENEMY TURN'
        
        DrawCards(DrawCount);
        Hand.ForEach(x => x.GetComponent<CardCombat>().selected = false);
        Hand.ForEach(x => x.transform.localScale = new Vector3(1,1,1));
        Debug.Log("New turn started. Cards in Deck, Hand, Discard: " + Deck.Count + "," + Hand.Count + "," + Discard.Count);
        txtDeck.GetComponent<Text>().text = "Deck:\n" + Deck.Count;
        txtDiscard.GetComponent<Text>().text = "Discard:\n" + Discard.Count;

        StartCoroutine(RulesSystem.instance.StartTurn());

        EnemiesInScene.ForEach(x => x.healthEffects.EffectsStartTurn());
    }

    IEnumerator WaitForAnimationsDiscardEndTurn(Action endArgument = null)
    {
        while (discardCardEndTurn.Count > 0)
        {
            yield return new WaitForSeconds(0.01f);
        }
        discardCardEndTurn.Clear();
        if(endArgument != null)
            endArgument.Invoke();
    }

    IEnumerator WaitForAnimationsDiscardsPlayerTurn(Action endArgument = null)
    {
        while (discardCardPlayerTurn.Count > 0)
        {
            Debug.Log(discardCardPlayerTurn.Count);
            yield return new WaitForSeconds(0.01f);
        }
        WinCombat();
        discardCardPlayerTurn.Clear();
        if(endArgument != null)
            endArgument.Invoke();
    }

    IEnumerator WaitForAnimationsDraw()
    {
        drawingCards = true;
        int i = 0;
        Debug.Log(Hand.Count);
        while (i < Hand.Count)
        {
            DisplayCard(i);
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Done");
        drawingCards = false;
        ResetSiblingIndexes();
    }

    public void CardDemarkTransition(GameObject gameObject)
    {
        discardCardEndTurn.Remove(gameObject);
    }

    public void DiscardCard(GameObject card)
    {
        Discard.Add(card);
        Hand.Remove(card);
    }

    public void DiscardCardToPile(GameObject card)
    {

        AnimateCardDiscard(card.GetComponent<CardCombat>());

        card.transform.localScale = GetCardScale();
        
        if(discardCardPlayerTurn.Count > 0)
            discardCardPlayerTurn.RemoveAt(0);
        txtDiscard.GetComponent<Text>().text = Discard.Count.ToString();
    }

    private void HideCard(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(-10000, -10000, -10000);
        gameObject.SetActive(false);
    }

    private void AnimateCardDiscard(CardCombat card)
    {
        card.inTransition = true;
        card.AnimateCardByPathDiscard();
    }

    private void AnimateCardDraw(CardCombat card, Vector3 endPos)
    {
        card.AnimateCardByCurve(endPos, true, false, true, true, true);
    }
    private void DisplayHand()
    {
        StartCoroutine(WaitForAnimationsDraw());
    }

    private void DisplayCard(int i)
    {
        Hand[i].SetActive(true);
        Hand[i].GetComponent<CardCombat>().transform.position = txtDeck.transform.position;
        Hand[i].GetComponent<CardCombat>().transform.localScale = Vector3.zero;
        AnimateCardDraw(Hand[i].GetComponent<CardCombat>(), GetPositionInHand(i));
    }

    public void ToggleInsidePanel()
    {
        if (mouseInsidePanel)
        {
            mouseInsidePanel = false;
        }
        else
        {
            mouseInsidePanel = true;
        }
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
        Debug.Log("Card used from Controller");
        targetedEnemy = enemy;
        if (ActiveCard is null)
            return;

        if((ActiveCard.cardData.targetRequired && enemy is null) || MouseInsideArea())
        {
            ActiveCard.DeselectCard();
            return;
        }

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


        foreach (CombatActorEnemy e in targetedEnemies) {
            RulesSystem.instance.CarryOutCard(ActiveCard.cardData,Hero, e);
            if (e.healthEffects.hitPoints <= 0)
                KillEnemy(e);
        } 

        discardCardPlayerTurn.Add(ActiveCard.gameObject);
        ActiveCard.UseCard();
        CheckVictory();
        ActiveCard = null;
    }

    private void KillEnemy(CombatActorEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        DeadEnemiesInScene.Add(enemy);
        EnemiesInScene.Remove(enemy);
    }

    public void CheckVictory()
    {
        Debug.Log(DeadEnemiesInScene.Count);
        if(DeadEnemiesInScene.Count == amountOfEnemies)
        {

            StartCoroutine(WaitForAnimationsDiscardsPlayerTurn());
        }
    }

    public void WinCombat()
    {
        //called from DEBUG
        while(EnemiesInScene.Count > 0)
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
        discardCardEndTurn.Clear();
        foreach (GameObject card in createdCards)
        {
            DestroyImmediate(card);
        }
    }
}
