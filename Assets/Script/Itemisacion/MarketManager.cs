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
    public List<Node> Stands;

    public int StandCorrutineCounter;
    public int AgentCorrutineCounter;


    public void ActualizeMarket()
    {

    }
   
}
