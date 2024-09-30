using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{

    public GameObject StatsCanvas;
    public GameObject SimCanvas;

    public void ToggleStatsObject()
    {
        StatsCanvas.SetActive(!StatsCanvas.activeSelf);
    }
    public void ToggleSimObject()
    {
        SimCanvas.SetActive(!SimCanvas.activeSelf);
    }
}
