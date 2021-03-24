using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Dialogue : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text textField;
    public Image playerImage;
    public Image participantImage;
    public Image nextImage;
    public Image doneImage;
    public TMP_Text playerText;
    public TMP_Text participantText;
    public bool sentenceDone;
    public float timeBetweenLetter;
    float speed;

    public void DisplaySentence(string aSentence)
    {
        StartCoroutine(TypeSentence(aSentence));
    }

    public void EndSentence()
    {
        speed = 0;
    }

    IEnumerator TypeSentence(string aSentence)
    {
        speed = timeBetweenLetter;
        sentenceDone = false;
        textField.text = "";
        foreach (char letter in aSentence)
        {
            textField.text += letter;
            if (letter.ToString() == "\n")
            {
                yield return new WaitForSeconds(speed * 20);
            }
            else if(letter.ToString() == ".")
            {
                yield return new WaitForSeconds(speed * 15);
            }
            else if(letter.ToString() == ",")
            {
                yield return new WaitForSeconds(speed * 10);
            }
            else
            {
                yield return new WaitForSeconds(speed);
            }
        }
        sentenceDone = true;
    }

    public void SetUI(Sprite aSprite, string aName, bool player)
    {
        if (player)
        {
            playerImage.gameObject.SetActive(true);
            participantImage.gameObject.SetActive(false);
            playerText.text = aName;
            playerImage.sprite = aSprite;
        }
        else
        {
            participantImage.gameObject.SetActive(true);
            playerImage.gameObject.SetActive(false);
            participantText.text = aName;
            participantImage.sprite = aSprite;
        }
    }
}
