using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour
{
    public string type;
    public Text textValue;
    private int defaultValue = 1;
    public int currentValue;
    private int minValue = 1;

    public StatsController statsController;

    public void IncrementStat()
    {
        int pt = statsController.RequestPoint();
        currentValue += pt;
        textValue.text = (currentValue).ToString();
    }

    public void DecrementStat()
    {
        if(currentValue > minValue)
        {
            
            currentValue = currentValue - 1;
            textValue.text = (currentValue).ToString();

            statsController.ReturnPoint();
        }
    }

    public void AddModifer()
    {
        if(statsController.aCharacterCreater.selectedChar != null)
        {
            List<StatModifer> allStats = statsController.aCharacterCreater.selectedChar.stats;

            foreach (var item in allStats)
            {
                if(type == item.type.ToString().ToLower())
                {
                    if(item.value > 0)
                    {
                        minValue = item.value;

                    }
                    currentValue += item.value;
                }  
            }
        }
    }

    public void ResetPoints()
    {
        currentValue = defaultValue;   
        AddModifer();
        textValue.text = (currentValue).ToString();
    }

    void Start()
    {
        ResetPoints();
    }
}
