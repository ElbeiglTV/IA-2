using FriendlyEditor.UtilityAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    #region Funcionamiento
    #region Singleton
    public static MarketManager instance;
    private void Awake() {if (instance == null) instance = this;else Destroy(this);}
    #endregion
    #region Market State
    public bool MarketIsOpen;
    public MarketState marketState;
    #endregion
    #region Scene Refs
    public LinqAgent AgentPrefab;
    public List<Node> Stands;
    public Node MarketEntrance;
    public Node MarketQueue;
    #endregion

    #region Internal Counters

    [DebugTag("Counter")]
    public int MarketQueueCounter;
    [DebugTag("Counter")]
    public int MarketEntranceCounter;
    [DebugTag("Counter")]
    public int StandCorrutineCounter;
    [DebugTag("Counter")]
    public int AgentCorrutineCounter;
    #endregion
    #region UI Refs
    public Button NextDayBT;
    public Button StatsBT;
    #endregion

    #endregion

    #region Stats


    public List<Tuple<Item, Stand>> DaySelledItems = new ();

    public List<Tuple<Item, Stand>> TotalSelledItems = new ();

    [DebugTag("SelledItems")]
    int TotalSelledItemsCounter => TotalSelledItems.Count;

    [DebugTag("SelledItems")]
    int DaySelledItemsCounter => DaySelledItems.Count;

    public int Days;


    #endregion



    private void Start()
    {
        MarketIsOpen = true;
        StartCoroutine(MarketCycle());
    }


    IEnumerator MarketCycle()
    {
        while (true)
        {
            Days++;
            marketState = MarketState.Selling;
            NextDayBT.gameObject.SetActive(false);   
            StatsBT.gameObject.SetActive(false);
            var random = UnityEngine.Random.Range(1, 6);
            foreach (var agents in LinqAgentGenerator().Take(random))
            {
                agents.InitializeAgent();
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Spawned "+ random +" Agents");

            while (AgentCorrutineCounter > 0)
            {
                yield return null;
            }
            MarketIsOpen = false;
            marketState = MarketState.ReStocking;
            foreach (var stand in Stands)
            {
                stand.GetComponent<Stand>().ReStock();
            }

            while (StandCorrutineCounter > 0)
            {
                yield return null;
            }

            if (DaySelledItems.Any())
            {
                if (TotalSelledItems.Any())
                {
                    Debug.LogWarning("DaySelledItems: "+DaySelledItems.Count + "TotalSelledItems: "+ TotalSelledItems.Count);

                    TotalSelledItems = TotalSelledItems.Concat(DaySelledItems).ToList();

                    Debug.LogWarning("DaySelledItems: " + DaySelledItems.Count + "TotalSelledItems: " + TotalSelledItems.Count);

                }
                else 
                {TotalSelledItems = DaySelledItems;}  
                

                yield return new WaitForSeconds(3);
                StatsManager.instance.DayEnd();
            }
            DaySelledItems = new List<Tuple<Item, Stand>>();

            marketState = MarketState.Closed;
            NextDayBT.gameObject.SetActive(true);
            StatsBT.gameObject.SetActive(true);
            while (!MarketIsOpen)
            {
                yield return null;
            }

            
        }
    }

   
    // Generators
    // Generate a list of agents  Utilize BuyListGenerator Gael Taborda
    public IEnumerable<LinqAgent> LinqAgentGenerator()
    {
        while (true)
        {
            LinqAgent LA = Instantiate(AgentPrefab);
            LA.itemsToBuy = BuyListGenerator().Take(UnityEngine.Random.Range(1, 20)).ToList();
            LA.money = UnityEngine.Random.Range(100, 2000);
            LA.ActualStand = 0;
            LA.gameObject.SetActive(true);
            yield return LA;
        }
    }
    // Generate a list of items to buy for the agent Matias Labreniuk
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

            item.itemType = types[UnityEngine.Random.Range(0,types.Length)];
            yield return item;
        }
    
    }

    public void MarketOpen()
    {
        MarketIsOpen = true;
    }




}


public enum MarketState
{
    ReStocking, Selling , Closed

}
