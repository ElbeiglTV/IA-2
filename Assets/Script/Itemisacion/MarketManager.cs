using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public static MarketManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public bool MarketIsOpen;
    public LinqAgent AgentPrefab;
    public MarketState marketState;
    public List<Node> Stands;
    public Node MarketEntrance;
    public Node MarketQueue;

    public int MarketQueueCounter;
    public int MarketEntranceCounter;


    public int StandCorrutineCounter;
    public int AgentCorrutineCounter;

    
    private void Start()
    {
        StartCoroutine(MarketCycle());
    }


    IEnumerator MarketCycle()
    {
        while (true)
        {
            foreach (var agents in LinqAgentGenerator().Take(Random.Range(0, 10)))
            {
                agents.InitializeAgent();
                yield return new WaitForSeconds(1);
            }

            while (AgentCorrutineCounter > 0)
            {
                yield return null;
            }
            MarketIsOpen = false;
            foreach (var stand in Stands)
            {
                stand.GetComponent<Stand>().ReStock();
            }

            while (StandCorrutineCounter > 0)
            {
                yield return null;
            }
            MarketIsOpen = true;
        }
    }


  
    public IEnumerable<LinqAgent> LinqAgentGenerator()
    {
        while (true)
        {
            LinqAgent LA = Instantiate(AgentPrefab);
            LA.itemsToBuy = BuyListGenerator().Take(Random.Range(1, 20)).ToList();
            LA.money = Random.Range(100, 2000);
            LA.ActualStand = 0;
            LA.gameObject.SetActive(true);
            yield return LA;
        }
    }
    public IEnumerable<NMItem> BuyListGenerator()
    {
        while (true)
        {
            NMItem item = new NMItem();
            string[] types = new[] {"Pescado azul", "Pescado rojo", "Pescado verde",
                        "Espada", "Escudo", "Hacha",
                        "Tarta", "Torta", "Pan", "Galletas",
                        "Banana", "Zanahoria", "Sandía",
                        "Carne", "Jamón", "Chorizo" };

            item.itemType = types[Random.Range(0,types.Length)];
            yield return item;
        }
    
    }


}


public enum MarketState
{
    ReStocking, Selling

}
