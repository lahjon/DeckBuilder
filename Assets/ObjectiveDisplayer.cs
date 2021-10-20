using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveDisplayer : MonoBehaviour
{
    public List<TMP_Text> objectiveDescriptions = new List<TMP_Text>();

    Queue<TMP_Text> freeDescriptions;

    public void Start()
    {
        freeDescriptions = new Queue<TMP_Text>(objectiveDescriptions);
    }

    public TMP_Text GetDescriptionSlot()
    {
        TMP_Text text = freeDescriptions.Dequeue();
        text.gameObject.SetActive(true);
        return text;
    }

    public void ReturnDescriptionSlot(TMP_Text text)
    {
        text.gameObject.SetActive(false);
        freeDescriptions.Enqueue(text);
    }

}
