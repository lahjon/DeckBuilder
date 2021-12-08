using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionIcon : MonoBehaviour, IToolTipable
{
    public ProfessionType profession;
    static float width = 50;
    public Transform tooltipAnchor;
    public (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = profession.GetDescription();
        return (new List<string>{desc} , pos, width);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
