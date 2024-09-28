using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LINQFunctions_ML
{
    //Funciones de LINQ de Matias Labreniuk

    public static List<Tuple<string,int,int>> TupleListCreator<T>(this List<Item> myItemList, int itemQuantity)
    {
        var myCollection = myItemList.Aggregate(new List<Tuple<string, int, int>>(), (acumulator, current) => 
                           {
                               var myTuple = Tuple.Create(current.itemType, current.price, itemQuantity);
                               acumulator.Add(myTuple);
                               return acumulator;
                           }
                           );
        return myCollection;
    }

    public static List<GameObject> PurchaseItems(this IEnumerable<Item> allItems)
    {
        return allItems.Where(x => x.active == false).OrderBy(x => x.cost).Select(x => x.gameObject).ToList();
    }
}
