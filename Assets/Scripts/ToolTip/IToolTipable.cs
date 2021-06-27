using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using UnityEngine.UI;


public interface IToolTipable
{
    (List<string> tips, Vector3 worldPosition) GetTipInfo();
}
