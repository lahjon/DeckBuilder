using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EncounterUITest : MonoBehaviour
{
    public EncounterDataTest encounterData;
    public TMP_Text encounterTitle;
    public TMP_Text encounterDescription;
    public GameObject canvas;
    public GameObject background;
    public GameObject panel;
    public GameObject event1;
    public GameObject event2;
    public GameObject event3;
    public GameObject choice1;
    public GameObject choice2;
    public GameObject choice3;
    public Encounter encounter;

    void Start()
    {
        //DEBUG
        BindEncounterData();
    }

    public void BindEncounterData()
    {

        if(encounterData.events.Count > 0)
        {
            event1 = encounterData.events[0];
            choice1.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice1;
            choice1.SetActive(true);
        }
        if(encounterData.events.Count > 1)
        {
            event2 = encounterData.events[1];
            choice2.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice2;
            choice2.SetActive(true);
        }
        if(encounterData.events.Count > 2)
        {
            event3 = encounterData.events[2];
            choice3.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice3;
            choice3.SetActive(true);
        }
        if(encounterTitle != null)
            encounterTitle.text = encounterData.name;
        if(encounterDescription != null)
            encounterDescription.text = encounterData.description;
    }
    public virtual void ConfirmOutcome(Reward aReward)
    {
        //WorldSystem.instance.characterManager.AddToCharacter(encounter.encounterData.encounterOutcome[index].type, encounter.encounterData.encounterOutcome[index].value);
    }
    public void CloseEncounter()
    {
        encounter.SetIsVisited(true);
    }

    public void ChooseOption(int index)
    {
        if (index == 1)
        {
            bool disable = event1.GetComponent<EventMain>().TriggerEvent();
            ConfirmOption(disable);
        }
        else if(index == 2)
        {
            bool disable = event2.GetComponent<EventMain>().TriggerEvent();
            ConfirmOption(disable);
        }
        else if(index == 3)
        {
            bool disable = event3.GetComponent<EventMain>().TriggerEvent();
            ConfirmOption(disable);
        }
    }

    public void ConfirmOption(bool disable)
    {
        if (disable)
        {
            canvas.SetActive(false);
        }
    }

    public void StartFade(GameObject oldObj, GameObject newObj)
    {
        StartCoroutine(FadeInAndOut(oldObj, newObj));
    }

    IEnumerator FadeInAndOut(GameObject oldObj, GameObject newObj)
    {
        CanvasGroup firstFade = oldObj.GetComponent<CanvasGroup>();
        CanvasGroup secondFade = newObj.GetComponent<CanvasGroup>();
        float time = 1.0f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            firstFade.alpha = Mathf.Abs(t - 1);
            yield return null;
        }
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            secondFade.alpha = t;
            yield return null;
        }
        DestroyImmediate(oldObj);
    }
}
