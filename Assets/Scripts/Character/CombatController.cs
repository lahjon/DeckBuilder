using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class CombatController : MonoBehaviour
{
    public GameObject TemplateCard;
    public TMP_Text lblEnergy;

    public Text txtDeck;
    public Text txtDiscard;
    public int HandSize = 10;
    public int DrawCount = 5;
    public float offset = 50;
    public int energyTurn = 3;

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

    public List<ActorDummy> EnemiesInScene = new List<ActorDummy>();

    [HideInInspector]
    public Card ActiveCard;
    [HideInInspector]
    public ActorDummy ActiveEnemy;

    void Start()
    {
        //AddCard();
        DeckData = DatabaseSystem.instance.GetStartingDeck();
        Debug.Log(DeckData.Count);
        DeckData.ForEach(x => Discard.Add(CreateCardFromData(x)));
        NextTurn();
    }
    // Update is called once per frame
    void Update()
    {
        /*
        Vector2 outCoordinates;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out outCoordinates);
        Debug.Log("Mouse:" + Input.mousePosition.x + "," + Input.mousePosition.y);
        Debug.Log(outCoordinates.x + "," + outCoordinates.y);
        Hand[2].GetComponent<RectTransform>().localPosition = outCoordinates;
        */
    }



    private void DisplayHand()
    {
        int n = Hand.Count;
        float width = GetComponent<RectTransform>().rect.width;
        float midPoint = width / 2;
        float localoffset = (n % 2 == 0) ? offset : 0;

        for (int i = 0; i < Hand.Count; i++)
        {
            Hand[i].transform.localPosition = new Vector3(midPoint + (i - n / 2) * 75 + localoffset, -75,0);
            Hand[i].SetActive(true);
        }

        ResetSiblingIndexes();
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.SetSiblingIndex(i);
    }

    //Denna m�ste �ndras till att blanda in discard om korten �r slut! 
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

            Hand.Add(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    private void ShuffleDeck()
    {
        for(int i = 0; i < Deck.Count; i++)
        {
            GameObject temp = Deck[i];
            int index = Random.Range(i, Deck.Count);
            Deck[i] = Deck[index];
            Deck[index] = temp;
        }
    }

    public bool CardisSelectable(Card card)
    {
        return ActiveCard is null && card.cardData.cost <= cEnergy;
    }


    GameObject CreateCardFromData(CardData cardData)
    {
        GameObject CardObject = Instantiate(TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(transform, false);
        CardObject.transform.localScale = new Vector3(.4f, .4f, .4f);
        Card Card = CardObject.GetComponent<Card>();
        Card.cardData = cardData;
        Card.combatController = this;
        Card.UpdateDisplay();
        HideCard(CardObject);
        return CardObject;
    }

    internal void CancelCardSelection(GameObject gameObject)
    {
        int n = Hand.Count;
        float width = GetComponent<RectTransform>().rect.width;
        float midPoint = width / 2;
        float localoffset = (n % 2 == 0) ? offset : 0;

        int i = Hand.IndexOf(gameObject);
        Hand[i].transform.localPosition = new Vector3(midPoint + (i - n / 2) * 75 + localoffset, -75, 0);
        ActiveCard = null;

        ResetSiblingIndexes();
    }

    public void NextTurn()
    {
        while (Hand.Count > 0)
            SendCardToDiscard(Hand[0]);

        DrawCards(DrawCount);
        DisplayHand();
        Debug.Log("New turn started. Cards in Deck, Hand, Discard: " + Deck.Count + "," + Hand.Count + "," + Discard.Count);
        txtDeck.text = "Deck:\n" + Deck.Count;
        txtDiscard.text = "Discard:\n" + Discard.Count;
        
        cEnergy = energyTurn;
        EnemiesInScene.ForEach(x => x.EffectsStartTurn());
    }

    public void SendCardToDiscard(GameObject card)
    {
        HideCard(card);
        Discard.Add(card);
        card.transform.localScale = new Vector3(.4f, .4f, .4f);
        Hand.Remove(card);
        txtDiscard.text = Discard.Count.ToString();
    }

    private void HideCard(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(-10000, -10000, -10000);
        gameObject.SetActive(false);
    }

    public void EnemyClicked(ActorDummy enemyDummy)
    {
        if (ActiveCard is null)
            return;

        cEnergy -= ActiveCard.cardData.cost;

        CardData cardData = ActiveCard.GetComponent<Card>().cardData;

        if(cardData.Effects.Count(x => x.Type == EffectType.Damage) > 0)
        {
            CardEffect damageComponent = cardData.Effects.Where(x => x.Type == EffectType.Damage).FirstOrDefault();
            for (int i = 0; i < damageComponent.Times; i++)
                enemyDummy.TakeDamage(damageComponent.Value);
        }

        cardData.Effects.Where(x => !(x.Type == EffectType.Damage || x.Type == EffectType.Block)).ToList().
            ForEach(x => enemyDummy.RecieveEffect(x));



        SendCardToDiscard(ActiveCard.gameObject);
        ActiveCard = null;

    }
}
