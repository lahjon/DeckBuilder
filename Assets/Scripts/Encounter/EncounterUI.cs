using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EncounterUI : MonoBehaviour
{
    public EncounterData encounterData;
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
    public EncounterData newEncounterData;

    void Start()
    {
 
    }

    public void StartEncounter()
    {
        canvas.SetActive(true);
        BindEncounterData();
    }

    public void BindEncounterData()
    {
        int count = 0;
        foreach (GameObject e in encounterData.events)
        {
            if (e != null)
            {
                count++;
            }
        }
        
        choice1.SetActive(false);
        choice2.SetActive(false);
        choice3.SetActive(false);

        if(count > 0)
        {
            choice1.SetActive(true);
            event1 = encounterData.events[0];
            
            choice1.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice1;
        }
        if(count > 1)
        {
            choice2.SetActive(true);
            event2 = encounterData.events[1];
            
            choice2.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice2;
        }
        if(count > 2)
        {
            choice3.SetActive(true);
            event3 = encounterData.events[2];
            
            choice3.transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choice3;
        }
        if(encounterTitle != null)
            encounterTitle.text = encounterData.name;
        if(encounterDescription != null)
            encounterDescription.text = encounterData.description;

    }

    public void CloseEncounter()
    {
        encounter.SetIsVisited(true);
    }

    public void ChooseOption(int index)
    {
        if (index == 1)
        {
            newEncounterData = encounterData.newEncounterData[0];
            bool disable = event1.GetComponent<EventMain>().TriggerEvent();
            ConfirmOption(disable);
        }
        else if(index == 2)
        {
            newEncounterData = encounterData.newEncounterData[1];
            bool disable = event2.GetComponent<EventMain>().TriggerEvent();
            ConfirmOption(disable);
        }
        else if(index == 3)
        {
            newEncounterData = encounterData.newEncounterData[2];
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
        float time = 0.6f;

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            firstFade.alpha = t / time;
            yield return null;
        }
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            secondFade.alpha = Mathf.Abs(1 - (t / time));
            yield return null;
        }
        DestroyImmediate(oldObj);
    }
}
