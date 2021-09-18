using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardIntLinkData
{
    public CardLinkablePropertyType linkedProp      = CardLinkablePropertyType.None;
    public int                      baseVal         = 0;
    public int                      scalar          = 1;
    public bool                     inverseScalar   = false;
}
