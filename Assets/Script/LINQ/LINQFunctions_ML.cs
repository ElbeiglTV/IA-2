using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LINQFunctions_ML
{
    //Funciones de LINQ de Matias Labreniuk

    public static List<Tuple<string,int, bool, Item>> TupleListCreator(this IEnumerable<Item> myItemList)
    {
        var myCollection = myItemList.Aggregate(new List<Tuple<string, int, bool, Item>>(), (acumulator, current) => 
                           {
                               if (current.active == true) 
                               { 
                                   var myTuple = Tuple.Create(current.itemType, current.price, current.active, current); 
                                   acumulator.Add(myTuple);
                               }
                               return acumulator;
                           }
                           );
        return myCollection;
    }

    public static IEnumerable<Item> PurchaseItems(this IEnumerable<Item> allItems)
    {
        return allItems.Where(x => x.active == false)           //Grupo 1: Where
                       .OrderBy(x => x.cost);                   //Grupo 2: OrderBy
    }

    public static IEnumerable<(string ItemType, int Count)> TupleTypeCounter(this List<Tuple<string, int, bool, Item>> allItems)
    {
        return allItems.GroupBy(x => x.Item1)                                           //GroupBy
                       .Select(group => (ItemType: group.Key, Count: group.Count()))    //Grupo 1: Select
                       .ToList();                                                       //Grupo 3: ToList
    }

    public static IEnumerable<(string ItemType, int LowestPrice)> GetLowestPricePerType(this List<Tuple<string, int, bool, Item>> allItems)
    {
        var lowestPrices = allItems
            .OrderBy(item => item.Item1)                                           // Grupo 2: OrderBy
            .ThenBy(item => item.Item2)                                            // Grupo 2: ThenBy
            .Select(item => new { item.Item1, item.Item2 })                        // Grupo 1: Select
            .GroupBy(item => item.Item1)                                           // GroupBy
            .Select(group => group.First())                                        // Grupo 1: Select & First
            .Select(result => (ItemType: result.Item1, LowestPrice: result.Item2)) // Grupo 1: Select
            .ToList();                                                             // Grupo 3: ToList

        return lowestPrices;
    }
}
