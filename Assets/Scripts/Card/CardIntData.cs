using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CardIntData
{
    public CardLinkablePropertyType linkedProp      = CardLinkablePropertyType.None;
    public int                      baseVal         = 0;
    public int                      scalar          = 1;
    public bool                     inverseScalar   = false;
}
