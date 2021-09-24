using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierFollow : MonoBehaviour
{

    public Transform routeDiscard;
    public Transform routeDeck;
    public Transform routeOath;
    private float tParam;
    private Vector3 objectPosition;
    private float speedModifier;
    private CardCombat attachedCard;

    private Vector3[] pathDiscard = new Vector3[4];
    private Vector3[] pathDeck = new Vector3[4];
    private Vector3[] pathOath = new Vector3[4];


    // Start is called before the first frame update
    void Start()
    {
        tParam = 0f;
        speedModifier = 1f;
        attachedCard = GetComponent<CardCombat>();

        for(int i = 1; i < 4; i++)
        {
            pathDiscard[i]  = routeDiscard.GetChild(i).position;
            pathOath[i]     = routeOath.GetChild(i).position;
            pathDeck[i]     = routeDeck.GetChild(i).position;
        }
    }

    public void StartAnimation(int path)
    {
        switch (path)
        {
            case 0:
                StartCoroutine(GoByTheRoute(pathDeck));
                break;
            case 1:
                StartCoroutine(GoByTheRoute(pathDiscard));
                break;
            case 2:
                StartCoroutine(GoByTheRoute(pathOath));
                break;
            default:
                break;
        }
    }

    private IEnumerator GoByTheRoute(Vector3[] p)
    {
        Vector3 startingAngles = transform.localEulerAngles;
        Vector3 endAngle = Vector3.zero;
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
            transform.localEulerAngles = Helpers.AngleLerp(startingAngles, endAngle, tParam);

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        CombatSystem.instance.UpdateDeckTexts();
        attachedCard.animator.SetTrigger("DoneDiscarding");
        tParam = 0f;
        attachedCard.GetComponent<Image>().raycastTarget = true;
    }
}
