using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierFollow : MonoBehaviour
{

    public Transform routeDiscard;
    public Transform routeDeck;
    private float tParam;
    private Vector3 objectPosition;
    private float speedModifier;
    private CardCombat attachedCard;

    private Vector3[] pathDiscard = new Vector3[4];
    private Vector3[] pathDeck = new Vector3[4];


    // Start is called before the first frame update
    void Start()
    {
        tParam = 0f;
        speedModifier = 1f;
        attachedCard = GetComponent<CardCombat>();

        for(int i = 1; i < 4; i++)
        {
            pathDiscard[i]  = routeDiscard.GetChild(i).position;
            pathDeck[i]     = routeDeck.GetChild(i).position;
        }
    }

    public void StartAnimation(bool toDiscard = true)
    {
        StartCoroutine(GoByTheRoute(toDiscard));
    }

    private IEnumerator GoByTheRoute(bool toDiscard = true)
    {
        Vector3 startingAngles = transform.localEulerAngles;
        Vector3 endAngle = Vector3.zero;

        Vector3[] p = toDiscard ? pathDiscard : pathDeck;
        p[0] = transform.position;

        float endScale = 0.7f;
        float scale;

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition =    1 * Mathf.Pow(1 - tParam, 3) * p[0] + 
                                3 * Mathf.Pow(1 - tParam, 2) * tParam * p[1] + 
                                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p[2] + 
                                1 * Mathf.Pow(tParam, 3) * p[3];

            scale = 1 - endScale * tParam;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.localEulerAngles = attachedCard.AngleLerp(startingAngles, endAngle, tParam);

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        WorldSystem.instance.combatManager.combatController.UpdateDeckTexts();
        attachedCard.animator.SetTrigger("DoneDiscarding");
        tParam = 0f;
        attachedCard.GetComponent<Image>().raycastTarget = true;
    }
}
