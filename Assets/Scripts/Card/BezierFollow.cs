// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class BezierFollow : MonoBehaviour
// {

//     public Transform routeDiscard;
//     public Transform routeDeck;
//     public Transform routeOath;
//     private float tParam = 0;
//     private float speedModifier = 1;

//     public Vector3[] pathDiscard = new Vector3[4];
//     public Vector3[] pathDeck = new Vector3[4];
//     public Vector3[] pathOath = new Vector3[4];


//     // Start is called before the first frame updates
//     void Start()
//     {
//         attachedCard = GetComponent<CardCombat>();
//         // routeDeck = routeOath = CombatSystem.instance.bezierController.parentDeck.transform;
//         // routeDiscard = routeDiscard = CombatSystem.instance.bezierController.parentDiscard.transform;
//         // routeOath = routeDeck = CombatSystem.instance.bezierController.parentSelf.transform;

//         // for(int i = 1; i < 4; i++)
//         // {
//         //     pathDiscard[i]  = routeDiscard.GetChild(i).position;
//         //     pathOath[i]     = routeOath.GetChild(i).position;
//         //     pathDeck[i]     = routeDeck.GetChild(i).position;
//         // }
//     }
// }
