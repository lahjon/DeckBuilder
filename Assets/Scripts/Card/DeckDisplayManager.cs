using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class DeckDisplayManager : Manager
{
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public GameObject deckDisplay;
    public CardVisual selectedCard;
    public ScrollRect scroller;
    public Vector3 previousPosition;
    public CardDisplay placeholderCard;
    public GameObject inspectCard;
    public Transform deckDisplayPos;
    public CardDisplay animatedCard;
    public TMP_Text titleText;
    public System.Action exitCallback;
    public TMP_Text upgradeLevel;
    public Toggle toggleUpgrade;
    public GameObject upgradeTools;


    protected override void Awake()
    {
        base.Awake(); 
        world.deckDisplayManager = this;
        canvas.gameObject.SetActive(true);
        inspectCard.SetActive(false);
        deckDisplay.SetActive(false);
    }

    public void OpenDisplay()
    {
        titleText.text = "All Cards";
        WorldStateSystem.SetInDisplay();
        WorldSystem.instance.deckDisplayManager.deckDisplay.SetActive(true);   
        WorldSystem.instance.deckDisplayManager.UpdateAllCards(0);
    }

    public void OpenUpgrade(System.Action callback)
    {
        titleText.text = "Pick a card to upgrade";
        exitCallback = callback;
        WorldStateSystem.SetInDisplay();
        WorldSystem.instance.deckDisplayManager.deckDisplay.SetActive(true);   
        WorldSystem.instance.deckDisplayManager.UpdateAllCards(1);  
    }

    /// <summary>
    /// <para>callbackTypes with int switch: </para>
    /// <para>0 = DisplayCard </para>
    /// <para>1 = UpgradeCard </para>
    /// </summary>
    public void UpdateAllCards(int callbackType)
    {
        int cardCount = world.characterManager.playerCards.Count;
        if(content.transform.childCount < cardCount)
        {
            while (content.transform.childCount < cardCount)
            {
                CardDisplay card = Instantiate(cardPrefab, content.transform).GetComponent<CardDisplay>();
                card.transform.SetParent(content.transform);
                card.gameObject.SetActive(true);
            }
        }
        else if(content.transform.childCount > cardCount)
        {
            while (content.transform.childCount > cardCount)
            {   
                Destroy(content.transform.GetChild(0).gameObject);
            }
        }

        for (int i = 0; i < content.transform.childCount; i++)
        {
            CardDisplay display = content.transform.GetChild(i).GetComponent<CardDisplay>();
            display.Mimic(world.characterManager.playerCards[i]);
            switch (callbackType)
            {
                case 0:
                    display.OnClick = () => DisplayCard(display);
                    break;
                case 1:
                    display.OnClick = () => {
                        if (world.characterManager.playerCards.FirstOrDefault(x => x.idx == display.idx)?.UpgradeCard() == true)
                        {
                            CloseDeckDisplay();
                            exitCallback?.Invoke();
                            exitCallback = null;
                        }
                        else
                            world.uiManager.UIWarningController.CreateWarning("Card is fully upgraded! Select another card!");
                    };
                    break;
                default:
                    break;
            }
        }
    }

    public void DisplayCard(CardVisual aCard)
    {
        if(selectedCard == null)
        {
            placeholderCard.Reset();
            aCard.OnMouseExit();
            previousPosition = aCard.transform.position;
            selectedCard = aCard;
            placeholderCard.Mimic(aCard);
            inspectCard.SetActive(true);
            scroller.enabled = false;
            upgradeLevel.text = (aCard.timesUpgraded + 1).ToString();
            selectedCard.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        }
        else
        {
            ResetCardPosition();
        }
    }

    public void ResetCard()
    {
        placeholderCard.Mimic(selectedCard);
        upgradeLevel.text = (selectedCard.timesUpgraded + 1).ToString();
    }
    

    public void ButtonNextUpgrade()
    {
        if (placeholderCard != null && placeholderCard.UpgradeCard())
        {
            upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
        }
    }
    public void ButtonPreviousUpgrade()
    {
        if (placeholderCard != null && placeholderCard.DowngradeCard())
        {
            upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
        }
    }
    public void ButtonToggleViewUpgrade()
    {
        if (toggleUpgrade.isOn)
        {
            upgradeTools.SetActive(true);
        }
        else
        {
            upgradeTools.SetActive(false);
            if (selectedCard != null && inspectCard.activeSelf)
            {
                ResetCard();
            }
        }
    }
    public void ResetCardPosition()
    {
        inspectCard.SetActive(false);
        toggleUpgrade.isOn = false;
        scroller.enabled = true;
        selectedCard.transform.position = previousPosition;
        selectedCard = null;
    }

    public void CloseDeckDisplay()
    {
        WorldStateSystem.TriggerClear();
        inspectCard.SetActive(false);
        deckDisplay.SetActive(false);
        selectedCard = null;
    }

    public void ButtonClose()
    {
        world.deckDisplayManager.CloseDeckDisplay();
    }

}
