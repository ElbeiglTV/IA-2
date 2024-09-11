using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public List<Node> grid = new();
    
    private IEnumerator Start()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        yield return wait;
        UpdateNeighbords();
    }

    public void UpdateNeighbords()
    {
        //Una opcion
        foreach (var node in grid)
        {
            node.neighbors.Clear();

            foreach (var otherNodes in grid)
            {
                if (node == otherNodes)
                    continue;

                if(GameManager.Instance.NodeInLineOfSight(node.transform.position, otherNodes.transform.position,node.nodeCollider,otherNodes.nodeCollider))  
                    node.neighbors.Add(otherNodes);
            }
        }

        //Otra opcion, tirar Raycast hacia los costados y adelante y atras
    }
    public Node GetNearesNode(Vector3 position) => grid.Select(node => new Tuple<float, Node>(Vector3.Distance(node.transform.position, position),node))
                                                       .OrderBy(x => x.Item1)
                                                       .First().Item2;

}
