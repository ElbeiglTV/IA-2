using System.Collections;
using System.Collections.Generic;
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
    public MarketState marketState;
    public List<Node> Stands;
    public Node MarketEntrance;
    public Node MarketQueue;

    public int MarketQueueCounter;


    public int StandCorrutineCounter;
    public int AgentCorrutineCounter;


    IEnumerator MarketCycle()
    {
        while(true)
        {











            foreach (var stand in Stands)
            {
                stand.GetComponent<Stand>().ReStock();
            }

            while (StandCorrutineCounter > 0)
            {
                yield return null;
            }
        }
    }



   
}
public enum MarketState
{
    ReStocking,Selling
   
}
