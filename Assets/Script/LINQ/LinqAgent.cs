using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FriendlyEditor.UtilityAttributes;

public class LinqAgent : MonoBehaviour
{
    public float QueueOfset;
    [DebugTag("Agent")]
    public float money;

    //[DebugTag("Agent"),DebugTag("ListasDeCompra")]
    public List<NMItem> itemsToBuy;

    public int ActualStand;

    public bool inSpawn;

    public void InitializeAgent()
    {
        MarketManager.instance.MarketEntranceCounter++;
        transform.position = MarketManager.instance.MarketEntrance.transform.position + new Vector3(0, QueueOfset * MarketManager.instance.MarketEntranceCounter, 0);
        inSpawn = true;

        if (MarketManager.instance.MarketIsOpen)
        {
            StartCoroutine(TravelAlongMarket());
        }

    }

    public IEnumerator TravelAlongMarket()
    {
        MarketManager.instance.AgentCorrutineCounter++;
        while (ActualStand < MarketManager.instance.Stands.Count)
        {


            if (MarketManager.instance.Stands[ActualStand].isOccupied)
            {

                MarketManager.instance.MarketQueueCounter++;
                transform.position = MarketManager.instance.MarketQueue.transform.position + new Vector3(0, QueueOfset * MarketManager.instance.MarketQueueCounter, 0);
                if (inSpawn == true)
                {
                    MarketManager.instance.MarketEntranceCounter--;
                    inSpawn = false;
                }
                if (ActualStand - 1 >= 0)
                {
                    MarketManager.instance.Stands[ActualStand - 1].isOccupied = false;
                }
                while (MarketManager.instance.Stands[ActualStand].isOccupied)
                {
                    yield return null;
                }
                MarketManager.instance.MarketQueueCounter--;
            }
            transform.position = MarketManager.instance.Stands[ActualStand].transform.position;
            if (inSpawn == true)
            {
                MarketManager.instance.MarketEntranceCounter--;
                inSpawn = false;
            }
            MarketManager.instance.Stands[ActualStand].isOccupied = true;
            if (ActualStand - 1 >= 0)
            {
                MarketManager.instance.Stands[ActualStand - 1].isOccupied = false;
            }
            yield return StartCoroutine(BuyItems(MarketManager.instance.Stands.Select(x => x.GetComponent<Stand>()).ToList()[ActualStand]));
            ActualStand++;
        }
        MarketManager.instance.Stands[ActualStand-1].isOccupied = false;

        MarketManager.instance.AgentCorrutineCounter--;

        Destroy(gameObject);
    }

    // Gael Taborda
    //Agrregate Doble
    public IEnumerator BuyItems(Stand standToBuy)
    {

        var StandItemsToBuy = itemsToBuy
        .Select(x => x.itemType) // selecciona el tipo de item
        .Aggregate(new Dictionary<string, int>(), (acc, x) =>
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
        }) // cuenta cuantos de cada tipo de item hay
        .Aggregate(new List<Item>(), (acc, x) =>
        {
            return acc.Concat(standToBuy.items.Where(y => y.itemType.Equals(x.Key) && y.active).OrderBy(z => z.price).Take(x.Value).ToList()).ToList();
        });// en base a el diccionario anterior selecciona los items a comprar de cada tipo y los agrega a una lista

        foreach (var item in StandItemsToBuy) // compra los items con 0.5 segundos de espera entre cada compra
        {
            if (money >= item.price)
            {
                money -= item.price;
                standToBuy.BuyItem(item);

                MarketManager.instance.DaySelledItems.Add(Tuple.Create<Item,Stand>(item, standToBuy));


                Debug.Log("Compre un " + item.itemType + " a " + item.price);
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

}
