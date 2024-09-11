using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue <T>
{
    Dictionary<T, float> _allElements = new Dictionary<T, float>();

    public int Count
    {
        get { return _allElements.Count; }
    }
    public void Enqueue(T elem, float cost)
    {
        _allElements.Add(elem, cost);
    }

    public T Dequeue()
    {
        float lowestValue = Mathf.Infinity;
        T elem = default;

        foreach (var item in _allElements)
        {
            if(item.Value < lowestValue)
            {
                lowestValue = item.Value;
                elem = item.Key;
            }
        }

        _allElements.Remove(elem);

        return elem;
    }
}
