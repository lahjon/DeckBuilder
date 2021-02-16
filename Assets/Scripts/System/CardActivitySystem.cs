using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardActivitySystem : MonoBehaviour
{
    public static CardActivitySystem instance = null;
    CombatController combatController;
    public Dictionary<CardActivityType, Func<string, IEnumerator>> ActivityTypeToAction = new Dictionary<CardActivityType, Func<string, IEnumerator>>();


    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    public void Start()
    {
        combatController = WorldSystem.instance.combatManager.combatController;
        ActivityTypeToAction[CardActivityType.DrawCard] = DrawCards;
        ActivityTypeToAction[CardActivityType.AddCardToDeck] = AddCardToDeck;
    }

    public IEnumerator StartByCardActivity(CardActivity cardActivity)
    {
        yield return StartCoroutine(ActivityTypeToAction[cardActivity.type].Invoke(cardActivity.parameter));
    }


    public IEnumerator DrawCards(string strX)
    {
        int x = Int32.Parse(strX);

        yield return StartCoroutine(combatController.DrawCards(x));
    }

    public IEnumerator AddCardToDeck(string cardId)
    {
        Debug.Log("Starting AddCard");
        CardData cd = DatabaseSystem.instance.cardDatabase.allCards.Where(x => x.name == cardId).FirstOrDefault();
        if(cd is null)
        {
            Debug.Log($"No such card named {cardId}");
            yield return null;
        }

        CardCombat card = combatController.CreateCardFromData(cd);
        combatController.Deck.Add(card);
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning($"Added card {cardId} to Deck!");
        combatController.UpdateDeckTexts();
    }

}
