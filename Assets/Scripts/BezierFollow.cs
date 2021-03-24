using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierFollow : MonoBehaviour
{

    public Transform route;

    private float tParam;

    private Vector3 objectPosition;

    private float speedModifier;

    CardCombat attachedCard;


    // Start is called before the first frame update
    void Start()
    {
        tParam = 0f;
        speedModifier = 1f;
        attachedCard = GetComponent<CardCombat>();
    }

    public void StartAnimation()
    {
        StartCoroutine(GoByTheRoute());
    }

    private IEnumerator GoByTheRoute()
    {
        Vector3 startingAngles = transform.localEulerAngles;
        Vector3 endAngle = Vector3.zero;
        Vector3 p0 = transform.position;
        Vector3 p1 = route.GetChild(1).position;
        Vector3 p2 = route.GetChild(2).position;
        Vector3 p3 = route.GetChild(3).position;

        float endScale = 0.7f;
        float scale;

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            scale = 1 - endScale * tParam;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.localEulerAngles = attachedCard.AngleLerp(startingAngles, endAngle, tParam);

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        WorldSystem.instance.combatManager.combatController.UpdateDeckTexts();
        attachedCard.animator.SetTrigger("DoneDiscarding");
        tParam = 0f;
    }
}
