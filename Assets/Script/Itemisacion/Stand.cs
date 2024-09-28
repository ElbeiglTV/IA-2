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
        Debug.Log("Bought " + item.itemType);
        
    }
}
