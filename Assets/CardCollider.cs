using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollider : MonoBehaviour
{
    public CardCombat card;

    public void OnMouseEnter() => card.OnMouseEnter();


    public void OnMouseExit() => card.OnMouseExit();

    public void OnMouseOver() => card.OnMouseOver();
}
