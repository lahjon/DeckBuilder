using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTest001 : Artifact
{
    public override void AddActivity()
    {
        Debug.Log("Add Activity");
    }

    public override void RemoveActivity()
    {
        Debug.Log("Remove Activity");
    }
}
