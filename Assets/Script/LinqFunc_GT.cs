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
 
}
