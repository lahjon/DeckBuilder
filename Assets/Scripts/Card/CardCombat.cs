using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombat : Card
{
    IEnumerator CardFollower;
    [HideInInspector]
    public CombatController combatController;

    void Awake()
    {
        CardFollower = FollowMouseIsSelected();
    }

    public override void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.ResetSiblingIndexes();
        }
    }

    public override void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

    private IEnumerator FollowMouseIsSelected()
    {
        while (true)
        {
            Vector2 outCoordinates;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(combatController.GetComponent<RectTransform>(), Input.mousePosition, null, out outCoordinates);
            GetComponent<RectTransform>().localPosition = outCoordinates;
            yield return new WaitForSeconds(0);
        }
    }
    IEnumerator LerpPosition(Vector3 endValue, float duration)
    {
        float time = 0;
        Vector3 startValue = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endValue;
        
        transform.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
    }

    public override void OnMouseClick()
    {
        if (!combatController.CardisSelectable(this))
            return;

        combatController.ActiveCard = this;
        StartCoroutine(CardFollower);
    }
    public override void OnMouseRightClick()
    {
        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
    }

}
