using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinqAgent : MonoBehaviour
{
    public float money;

    public List<NMItem> itemsToBuy;

    public int ActualStand;

    public void InitializeAgent()
    {
        transform.position = MarketManager.instance.MarketEntrance.transform.position;

        if (MarketManager.instance.MarketIsOpen)
        {
        StartCoroutine(TravelAlongMarket());
        }

    }

    public IEnumerator TravelAlongMarket()
    {
        MarketManager.instance.AgentCorrutineCounter++;
        while(ActualStand < MarketManager.instance.Stands.Count)
        {


            if (MarketManager.instance.Stands[ActualStand].isOccupied)
            {
                
                MarketManager.instance.MarketQueueCounter++;
                transform.position = MarketManager.instance.MarketQueue.transform.position + new Vector3(0,10*MarketManager.instance.MarketQueueCounter,0);
                while (MarketManager.instance.Stands[ActualStand].isOccupied)
                {
                    yield return null;
                }
            }
            transform.position = MarketManager.instance.Stands[ActualStand].transform.position;
            MarketManager.instance.Stands[ActualStand].isOccupied = true;
            MarketManager.instance.Stands[ActualStand-1].isOccupied = false;
            yield return StartCoroutine(BuyItems(MarketManager.instance.Stands.Select(x => x.GetComponent<Stand>()).ToList()[ActualStand]));
            ActualStand++;
        }

        MarketManager.instance.AgentCorrutineCounter--; 
        Destroy(gameObject);
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
