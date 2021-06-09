using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class EncounterEdge
{
    public Encounter n1;
    public Encounter n2;

    public EncounterEdge(Encounter n1, Encounter n2)
    {
        this.n1 = n1;
        this.n2 = n2;
    }

    public (Vector2 v1, Vector2 v2) GetNodePos() => (n1.transform.localPosition, n2.transform.localPosition);

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            EncounterEdge other = (EncounterEdge)obj;
            return (n1 == other.n1 && n2 == other.n2) || (n1 == other.n2 && n2 == other.n1);
        }
    }

    public bool Equals(Encounter n1, Encounter n2)
    {
        return (n1 == this.n1 && n2 == this.n2) || (n1 == this.n2 && n2 == this.n1);
    }
}
