using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class EncounterUI : MonoBehaviour
{
    public EncounterDataRandomEvent encounterData;
    public TMP_Text encounterTitle;
    public TMP_Text encounterDescription;
    public GameObject canvas;
    public GameObject background;
    public GameObject panel;
    public GameObject choice1;
    public GameObject choice2;
    public GameObject choice3;
    public Encounter encounter;
    public EncounterData newEncounterData;
    private bool transition = false;

    private GameObject[] choices;

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
        GameObject UI = Instantiate(background, background.transform.position, Quaternion.Euler(0, 0, 0), panel.transform);
        return UI;
    }

    public void BindEncounterData()
    {
        encounterTitle.text = encounterData.name;
        encounterDescription.text = encounterData.description;

        foreach (GameObject go in choices)
            go.SetActive(false);

        for (int i = 0; i < encounterData.choices.Count; i++)
        {
            choices[i].SetActive(true);
            choices[i].transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choices[i].label;
        }
    }

    public void CloseEncounter()
    {
        //StartCoroutine(encounter.Entering(() => { }));
        canvas.SetActive(false);
    }

    public void ChooseOption(int index)
    {
        newEncounterData = encounterData.choices[index -1].newEncounter;
        bool disable = EncounterEventResolver.TriggerEvent(encounterData.choices[index-1].outcome);
        ConfirmOption(disable);

        if (newEncounterData is EncounterDataCombat encC)
            WorldSystem.instance.combatManager.combatController.encounterData = encC;
        else if (newEncounterData is EncounterDataRandomEvent encR)
            WorldSystem.instance.uiManager.encounterUI.encounterData = encR;
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
