using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EncounterUI : MonoBehaviour
{
    public EncounterDataRandomEvent encounterData;
    public TMP_Text encounterTitle;
    public TMP_Text encounterDescription;
    public GameObject canvas;
    public GameObject background;
    public GameObject panel;
    public EncounterEventType event1;
    public EncounterEventType event2;
    public EncounterEventType event3;
    public GameObject choice1;
    public GameObject choice2;
    public GameObject choice3;
    public Encounter encounter;
    public EncounterData newEncounterData;
    private bool transition = false;

    private GameObject[] choices;
    private string[] encounterDateChoices = new string[3];

    void Start()
    {
        choices = new GameObject[] { choice1, choice2, choice3 };
    }

    public void StartEncounter()
    {
        canvas.SetActive(true);
        BindEncounterData();
    }

    public GameObject CreateNewUI(GameObject background, GameObject panel)
    {
        GameObject oldUI = Instantiate(background, background.transform.position, Quaternion.Euler(0, 0, 0), panel.transform) as GameObject;
        return oldUI;
    }

    public void ResetEncounter()
    {
        encounterData = null;
        newEncounterData = null;
    }

    public void BindEncounterData()
    {

        int count = 0;
        foreach (EncounterEventType e in encounterData.events)
            if (e != EncounterEventType.None)
                count++;
        
        choice1.SetActive(false);
        choice2.SetActive(false);
        choice3.SetActive(false);

        encounterDateChoices[0] = encounterData.choice1;
        encounterDateChoices[1] = encounterData.choice2;
        encounterDateChoices[2] = encounterData.choice3;

        for (int i = 0; i < count; i++)
        {
            choices[i].SetActive(true);
            choices[i].transform.GetChild(0).GetComponent<TMP_Text>().text = encounterDateChoices[i];
        }


        encounterTitle.text = encounterData.name;
        encounterDescription.text = encounterData.description;

    }

    public void CloseEncounter()
    {
        //StartCoroutine(encounter.Entering(() => { }));
        canvas.SetActive(false);
    }

    public void ChooseOption(int index)
    {
        newEncounterData = encounterData.newEncounterData[index -1];
        if (newEncounterData is EncounterDataCombat encC)
            WorldSystem.instance.combatManager.combatController.encounterData = encC;
        else if (newEncounterData is EncounterDataRandomEvent encR)
            WorldSystem.instance.uiManager.encounterUI.encounterData = encR;

        bool disable = EncounterEventResolver.TriggerEvent(encounterData.events[index-1]);
        ConfirmOption(disable);
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
        transition = true;
        CanvasGroup firstFade = oldObj.GetComponent<CanvasGroup>();
        CanvasGroup secondFade = newObj.GetComponent<CanvasGroup>();
        float time = 0.3f;

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
        transition = false;
        DestroyImmediate(oldObj);
    }
}
