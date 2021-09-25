using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierController : MonoBehaviour
{
    public Transform parentDiscard;
    public Transform parentDeck;
    public Transform parentSelf;
    public Vector3[] routeDeck;
    public Vector3[] routeDiscard;
    public Vector3[] routeSelf;

    void Start()
    {
        routeDeck = new Vector3[4]; 
        routeDiscard = new Vector3[4]; 
        routeSelf = new Vector3[4]; 

        for(int i = 1; i < 4; i++)
        {
            routeDeck[i]    = parentDeck.GetChild(i).position;
            routeDiscard[i] = parentDiscard.GetChild(i).position;
            routeSelf[i]    = parentSelf.GetChild(i).position;
        }
    }
}
