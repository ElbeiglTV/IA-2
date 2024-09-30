using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    
    void Start()
    {
        StatsManager.instance.OnDayEndUIStats.AddListener(SelfDestroy);
    }
    public void SelfDestroy(List<Tuple<string,int,float,string>> z, List<Tuple<string, int, float, string>> x)
    {
       Destroy(gameObject);
    }

}
