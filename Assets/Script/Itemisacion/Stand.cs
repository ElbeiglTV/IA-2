using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stand : MonoBehaviour
{
    public float money = 1000; // the money of the stand
    public List<GameObject> standObjects; // the stand objects

    // list of Visual objects for this stand (GameObject)
    public List<MeshRenderer> visuals => standObjects.Select(x => x.GetComponentInChildren<MeshRenderer>()).ToList();
    // list of Items for this stand (Item)
    public IEnumerable<Item> items => standObjects.Select(x => x.GetComponentInChildren<Item>());
    public List<Tuple<string, int, bool, Item>> tupleList => items.TupleListCreator();

    private void Start()
    {
        InitializeStand();
    }
    
    void InitializeStand()
    {
        foreach (var item in items)
        {
            item.active = true;
        }
        foreach (var visual in visuals)
        {
            visual.enabled = true;
        }
    }
    public void DeInitializeStand()
    {
        foreach (var item in items)item.active = false;
        foreach (var visual in visuals) visual.enabled = false;
        
           
        
    }
    public void BuyItem(Item item)
    {
        money += item.price;
        item.active = false;
        visuals[items.ToList().IndexOf(item)].enabled = false;   
    }

    public void ReStock()
    {
        StartCoroutine(Purchase());

    }

    //Time-Slicing Matias Labreniuk

    public IEnumerator Purchase()
    {
        MarketManager.instance.StandCorrutineCounter++;
        foreach (var item in items.PurchaseItems())
        {
            if (money >= item.cost)
            {
                money -= item.cost;
                item.active = true;
                visuals[items.ToList().IndexOf(item)].enabled = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
        MarketManager.instance.StandCorrutineCounter--;
    }

    
}
