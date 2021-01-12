using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
    public string type;
    public Text textValue;
    private int defaultValue = 5;
    public int currentValue;

    public GameObject statsController;
    

    public void IncrementStat()
    {
        int pt = statsController.GetComponent<StatsController>().RequestPoint();
        currentValue += pt;
        textValue.text = (currentValue).ToString();
    }

    public void DecrementStat()
    {
        
        if(currentValue > 1)
        {
            
            currentValue = currentValue - 1;
            textValue.text = (currentValue).ToString();

            statsController.GetComponent<StatsController>().ReturnPoint();
        }
    }

    public void ResetPoints()
    {
        currentValue = defaultValue;   
        textValue.text = (currentValue).ToString();
    }

    void Start()
    {
        ResetPoints();
    }
}
