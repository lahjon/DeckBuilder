using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldStartup : MonoBehaviour
{
    // this class will always have its start function last
    void Start()
    {
        WorldSystem.instance.SaveProgression();
    }
}
