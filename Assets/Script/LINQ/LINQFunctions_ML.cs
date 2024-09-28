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
                               var myTuple = Tuple.Create(current.itemType, current.price, current.active, current);
                               acumulator.Add(myTuple);
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

    public static IEnumerable<dynamic> TupleTypeCounter(this List<Tuple<string, int, bool, Item>> allItems)
    {
        var itemCount = allItems.GroupBy(x => x.Item1)                                              //GroupBy
                                .Select(group => new {Item1 = group.Key, Count = group.Count()})    //Grupo 1: Select
                                .ToList();                                                          //Grupo 3: ToList
        return itemCount;
    }
}
