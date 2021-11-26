using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUI : MonoBehaviour, IToolTipable, IEventSubscriber
{
    Stat stat;
    public StatType statType;
    public Image image;
    public Transform tooltipAnchor;
    string displayText;
    public TMP_Text valueText;

    void Start()
    {
        stat = WorldSystem.instance.characterManager.characterStats.GetStat(statType);
        GetAllStatModifiers();
        Subscribe();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void GetAllStatModifiers()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("<b>{0}</b>\n", statType.ToString()));
        string statText = "";
        for (int i = 0; i < stat.StatModifers.Count; i++)
        {
            string source = stat.StatModifers[i].GetName();
            int value = stat.StatModifers[i].GetValue();
            string valueStr = value >= 0 ? string.Format("+ {0}", value) : string.Format("- {0}", System.Math.Abs(value));
            if (i < stat.StatModifers.Count)
                statText = string.Format("{0}: {1}\n", source, valueStr);
            else
                statText = string.Format("{0}: {1}", source, valueStr);
            sb.Append(statText); 
        }
        displayText = sb.ToString();
        valueText.text = stat.value.ToString();
    }
    public virtual (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.position);
        return (new List<string>{displayText} , pos);
    }

    public void Subscribe()
    {
        EventManager.OnStatModifiedEvent += GetAllStatModifiers;
    }

    public void Unsubscribe()
    {
        EventManager.OnStatModifiedEvent -= GetAllStatModifiers;
    }
}