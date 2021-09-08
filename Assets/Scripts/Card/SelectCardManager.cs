// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SelectCardManager : MonoBehaviour
// {
//     public GameObject canvas;
//     public List<CardDisplay> allDisplayedCards;
//     public GameObject cardPrefab;

//     // void AdjustCards()
//     // {

//     // }


//     public void CreateCards(CardLocation cardLocation)
//     {
//         List<CardVisual> allCards = new List<CardVisual>();
//         allCards.Clear();
//         switch (cardLocation)
//         {
//             case CardLocation.Deck:
//                 break;
            
//             default:
//                 break;
//         }
//         if (type == DeckType.CombatDeck)
//             CombatSystem.instance.Hero.deck.ForEach(c => sourceCards.Add((CardVisual)c));
//         else
//             CombatSystem.instance.Hero.discard.ForEach(c => sourceCards.Add((CardVisual)c));
//     }
// }
