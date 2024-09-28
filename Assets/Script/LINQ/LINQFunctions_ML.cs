using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LINQFunctions_ML
{
    //Funciones de LINQ de Matias Labreniuk

    public static List<Tuple<string,int>> TupleListCreator<T>(this List<Item> myItemList)
    {
        var myCollection = myItemList.Aggregate(new List<Tuple<string, int>>(), (acumulator, current) => 
                           {
                               var myTuple = Tuple.Create(current.itemType, current.price);
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
}
