using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierFollow : MonoBehaviour
{

    public Transform route;

    private float tParam;

    private Vector3 objectPosition;

    private float speedModifier;


    // Start is called before the first frame update
    void Start()
    {
        tParam = 0f;
        speedModifier = 8f;
    }

    public void StartAnimation()
    {
        StartCoroutine(GoByTheRoute());
    }

    private IEnumerator GoByTheRoute()
    {

        Vector2 p0 = transform.position;
        Vector2 p1 = route.GetChild(1).position;
        Vector2 p2 = route.GetChild(2).position;
        Vector2 p3 = route.GetChild(3).position;

        float endScale = 0.7f;
        float scale = 1;

        while(tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            scale = 1 - endScale * tParam;
            transform.localScale = new Vector3(scale, scale, scale);

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        bool waiting = true;
        while(waiting)
        {
            waiting = false;
            yield return new WaitForSeconds(0.1f);
        }

        tParam = 0f;
        this.GetComponent<CardCombat>().selected = false;
        this.GetComponent<CardCombat>().inTransition = false;
        this.GetComponent<CardCombat>().combatController.CheckInTransition(false);

        transform.localScale = new Vector3(1,1,1);
        this.gameObject.SetActive(false);

    }
}
