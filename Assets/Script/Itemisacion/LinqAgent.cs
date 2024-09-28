using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinqAgent : MonoBehaviour
{
    public float money;

    public List<Item> itemsToBuy;

    public List<Node> Stands;
    public int ActualStand;

    private void Start()
    {
        if (MarketManager.instance.MarketIsOpen)
        {
        StartCoroutine(TravelAlongMarket());
        }
    }

    public IEnumerator TravelAlongMarket()
    {
        MarketManager.instance.AgentCorrutineCounter++;
        while(ActualStand < Stands.Count)
        {
            transform.position = Stands[ActualStand].transform.position;
            yield return StartCoroutine(BuyItems(Stands.Select(x => x.GetComponent<Stand>()).ToList()[ActualStand]));
            ActualStand++;
        }
        MarketManager.instance.AgentCorrutineCounter--; 
    }
    public IEnumerator BuyItems(Stand standToBuy)
    {

        var StandItemsToBuy = itemsToBuy.Select(x => x.itemType).Aggregate(new Dictionary<string, int>(), (acc, x) =>
        {
            if (acc.ContainsKey(x))
            {
                acc[x]++;
            }
            else
            {
                acc.Add(x, 1);
            }
            return acc;
        }).Aggregate(new List<Item>(), (acc, x) =>
        {
            return acc.Concat(standToBuy.items.Where(y => y.itemType.Equals(x.Key) && y.active).OrderBy(z => z.price).Take(x.Value).ToList()).ToList();
        });

        foreach (var item in StandItemsToBuy)
        {
            if (money >= item.price)
            {
                money -= item.price;
                standToBuy.BuyItem(item);

                Debug.Log("Compre un " + item.itemType + " a " +item.price);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
