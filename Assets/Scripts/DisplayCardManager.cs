using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DisplayCardManager : Manager
{
    public GameObject canvas, cardDisplayPrefab;
    public Transform content;
    public List<CardDisplay> allCards = new List<CardDisplay>();
    public TMP_InputField inputField;
    public TMP_Dropdown classDropdown;
    public bool active;

    protected override void Awake()
    {
        base.Awake(); 
        world.displayCardManager = this;
    }
    protected override void Start()
    {
        base.Start();
        AddAllCards();
    }
    void AddAllCards()
    {
        List<CardData> allCardDatas = new List<CardData>();
        List<CardData> allCardsEnemies = new List<CardData>();
        List<CardData> allCardsPlayer = new List<CardData>();
        List<CardData> allCardsOther = new List<CardData>();
        
        foreach (CardData card in DatabaseSystem.instance.cards)
        {
            if (card.cardClass == CardClassType.Enemy)
                allCardsEnemies.Add(card);
            else if(card.cardClass == CardClassType.None || card.cardClass == CardClassType.Torment || card.cardClass == CardClassType.Burden)
                allCardsOther.Add(card);
            else
                allCardsPlayer.Add(card);
        }

        allCardsPlayer = allCardsPlayer.OrderBy(x => x.cardName).ToList();
        allCardsEnemies = allCardsEnemies.OrderBy(x => x.cardName).ToList();
        allCardsOther = allCardsOther.OrderBy(x => x.cardName).ToList();
        allCardDatas.AddRange(allCardsPlayer);
        allCardDatas.AddRange(allCardsOther);
        allCardDatas.AddRange(allCardsEnemies);

        foreach (CardData card in allCardDatas)
        {
            CardDisplay cardDisplay = Instantiate(cardDisplayPrefab, content).GetComponent<CardDisplay>();
            cardDisplay.cardData = card;
            cardDisplay.BindCardData();
            cardDisplay.BindCardVisualData();
            if ((int)card.cardClass < 5)
            {
                cardDisplay.OnClick = () =>  {
                    world.characterManager.AddCardToDeck(cardDisplay);
                    world.uiManager.UIWarningController.CreateWarning(string.Format("{0} added to your deck!", cardDisplay.cardName));
                };
            }
            allCards.Add(cardDisplay);
        }

        // List<string> optionsClass = new List<string>();
        // optionsClass.Add("Any");
        // optionsClass.Add("Berserker");
        // optionsClass.Add("Splicer");
        // optionsClass.Add("Beastmaster");
        // optionsClass.Add("Rogue");
        // optionsClass.Add("Other");
        // optionsClass.Add("Enemies");
        // DatabaseSystem.instance.encounterEvent.ForEach(x => optionsClass.Add(x.name));
        // classDropdown.AddOptions(optionsClass);
    }
    public void UpdateFilter()
    {
        string current = classDropdown.options[classDropdown.value].text;
        foreach (var item in allCards)
        {
            if (!item.cardName.ToLower().Contains(inputField.text.ToLower()))
                item.gameObject.SetActive(false);
            else
                item.gameObject.SetActive(true);
        }

        
    }
    public void ButtonClose()
    {
        canvas.SetActive(false);
        inputField.text= "";
        active = false;
    }
    public void ButtonOpen()
    {
        canvas.SetActive(true);
        active = true;
    }
}
