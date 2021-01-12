using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StatsController : MonoBehaviour
{
    private int defaultStartingStatPoints = 5;
    public int statPoints;
    public CharacterCreator aCharacterCreater;
    public Text pointValue;
    public List<StatController> statControllers;

    void Start()
    {
        ResetPoints();
    }

    public int RequestPoint()
    {
        if(statPoints > 0)
        {
            statPoints--;
            pointValue.text = statPoints.ToString();
            return 1;
        }
        else
            return 0;
    }

    public void ReturnPoint()
    {
        statPoints++;
        pointValue.text = statPoints.ToString();
    }

    public void ResetPoints()
    {
        statPoints = defaultStartingStatPoints;
        statControllers.ForEach(x => x.ResetPoints());
        pointValue.text = statPoints.ToString();
    }




    public Dictionary<string, int> FetchStats()
    {
        Dictionary<string, int> characterStats = new Dictionary<string, int>();

        foreach (StatController x in statControllers)
        {
            characterStats[x.type] = x.currentValue;
        }

        return characterStats;
    }
}
