using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class LinqFunc_GT
{
  // funciones con linq de Gael Taborda

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Random.value);
    }
    public static IEnumerable<T> InspectItems<T>(this IEnumerable<T> items, int costFilter) where T : Item
    {
        return items.Where(x => x.active && x.price < costFilter).OfType<T>();
    }
    public static float BuyItems<T>(this float money, IEnumerable<T> buyList,Stand stand) where T : Item
    {
        float totalCost = buyList.Aggregate(0f, (acc, x) => acc + x.price);

        if(money >= totalCost)
        {
           foreach (var item in buyList)
           {
               item.active = false;
           }
           stand.money += totalCost;
           return money -= totalCost;
        }
        else
        {
            return money;
        }
    }





}
