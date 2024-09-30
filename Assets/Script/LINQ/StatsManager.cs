using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
   public static StatsManager instance;
    private void Awake() {if (instance == null) instance = this;else Destroy(this);}

    public StatsEvent OnDayEndUIStats;

    public void DayEnd()
    {
        Debug.Log("DayEnd");

        var FilteredDayStats = MarketManager.instance.DaySelledItems
            .Select(x => Tuple.Create(new { x.Item1.itemType, itemPrise = x.Item1.price }, x.Item2.StandType)) // Parcial Grupo 1
            .GroupBy(x => x.Item1.itemType)
            .Select(group => Tuple.Create(group.Key, group.Count(), group.Aggregate(0f, (acc, j) => acc + j.Item1.itemPrise), group.ElementAt(0).Item2))// Parcial Grupo 1
            .OrderByDescending(x => x.Item2)// Parcial Grupo 2
            .ToList();// Parcial Grupo 3

        var filteredTotalStats = MarketManager.instance.TotalSelledItems
            .Select(x => Tuple.Create(new { x.Item1.itemType, itemPrise = x.Item1.price }, x.Item2.StandType)) // Parcial Grupo 1
            .GroupBy(x => x.Item1.itemType)
            .Select(group => Tuple.Create(group.Key, group.Count(), group.Aggregate(0f, (acc, j) => acc + j.Item1.itemPrise), group.ElementAt(0).Item2))// Parcial Grupo 1
            .OrderByDescending(x => x.Item2) // Parcial Grupo 2
            .ToList();// Parcial Grupo 3

        OnDayEndUIStats.Invoke(FilteredDayStats,filteredTotalStats);
    }
}
[System.Serializable]
public class  StatsEvent : UnityEvent<List<Tuple<string,int,float,string>>, List<Tuple<string, int, float, string>>>
{
    
}
