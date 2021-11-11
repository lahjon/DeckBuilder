using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollider : MonoBehaviour
{
    public CardCombat card;

    // Update is called once per frame
    void Update()
    {
        (Vector3 position, Vector3 angles) posInfo = CombatSystem.instance.GetPositionInHand(card);
    }
}
