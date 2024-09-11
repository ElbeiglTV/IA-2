using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pathfinding : MonoBehaviour
{
    public List<Node> CalculateBFS(Node startingNode, Node goalNode)
    {
        var frontier = new Queue<Node>();
        frontier.Enqueue(startingNode);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if(current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

            foreach (var item in current.GetNeighbors) 
            {
                if (!cameFrom.ContainsKey(item))
                {
                    frontier.Enqueue(item);
                    cameFrom.Add(item, current);
                }
            }
        }

        return new List<Node>();
    }

    public List<Node> CalculateGreedyBFS(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

            foreach (var item in current.GetNeighbors)
            {
                if (!cameFrom.ContainsKey(item))
                {
                    float priority = Vector3.Distance(item.transform.position, goalNode.transform.position);    
                    frontier.Enqueue(item, priority);
                    cameFrom.Add(item, current);
                }
            }
        }

        return new List<Node>();
    }

    public List<Node> CalculateDijkstra(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

            foreach (var item in current.GetNeighbors)
            {

                int newCost = costSoFar[current] + item.Cost;

                if (!costSoFar.ContainsKey(item))
                {
                    costSoFar.Add(item, newCost);
                    frontier.Enqueue(item, newCost);
                    cameFrom.Add(item, current);
                }else if (costSoFar[item] > newCost)
                {
                    frontier.Enqueue(item, newCost);
                    cameFrom[item] = current;
                    costSoFar[item] = newCost;
                }
            }
        }

        return new List<Node>();
    }

    public List<Node> CalculateAStar(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

            foreach (var item in current.GetNeighbors)
            {
                float newCost = costSoFar[current] + item.Cost;
                float priority = newCost + Vector3.Distance(item.transform.position, goalNode.transform.position);

                if (!costSoFar.ContainsKey(item))
                {
                    costSoFar.Add(item, newCost);
                    frontier.Enqueue(item, priority);
                    cameFrom.Add(item, current);
                }
                else if (costSoFar[item] > newCost)
                {
                    frontier.Enqueue(item, priority);
                    cameFrom[item] = current;
                    costSoFar[item] = newCost;
                }
            }
        }

        return new List<Node>();
    }

    public List<Node> CalculateTheta(Node startingNode, Node goalNode)
    {
        var listNode = CalculateAStar(startingNode, goalNode);

        int current = 0;

        while (current + 2 < listNode.Count)
        {
            if (GameManager.Instance.InLineOfSight(listNode[current]. transform.position, listNode[current + 2].transform.position))
                listNode.RemoveAt(current + 1);
            else
                current++;
        }

        return listNode;
    }

    public IEnumerator CoroutineTheta(Node startingNode, Node goalNode)
    {
        var listNode = CalculateAStar(startingNode, goalNode);

        foreach (var item in listNode)
            item.GetComponent<MeshRenderer>().material.color = Color.yellow;

        int current = 0;

        while (current + 2 < listNode.Count)
        {
            listNode[current].GetComponent<MeshRenderer>().material.color = Color.cyan;
            listNode[current + 2].GetComponent<MeshRenderer>().material.color = Color.green;

            yield return new WaitForSeconds(0.4f);

            if (GameManager.Instance.InLineOfSight(listNode[current].transform.position, listNode[current + 2].transform.position))
            {
                listNode[current + 1].GetComponent<MeshRenderer>().material.color = Color.red;

                yield return new WaitForSeconds(0.4f);

                listNode[current + 1].GetComponent<MeshRenderer>().material.color = Color.magenta;

                listNode.RemoveAt(current + 1);
            }
            else
            {
                listNode[current].GetComponent<MeshRenderer>().material.color = Color.cyan;
                current++;
            }
        }
    }

    public IEnumerator CoroutineDijkstra(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();

                foreach (var item in path)
                {
                    item.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    yield return new WaitForSeconds(0.05f);
                }

                yield break;
            }

            foreach (var item in current.GetNeighbors)
            {
                int newCost = costSoFar[current] + item.Cost;

                item.GetComponent<MeshRenderer>().material.color = Color.blue;
                yield return new WaitForSeconds(0.05f);

                if (!costSoFar.ContainsKey(item))
                {
                    costSoFar.Add(item, newCost);
                    frontier.Enqueue(item, newCost);
                    cameFrom.Add(item, current);
                }
                else if (costSoFar[item] > newCost)
                {
                    frontier.Enqueue(item, newCost);
                    cameFrom[item] = current;
                    costSoFar[item] = newCost;
                }
            }
        }
    }

    public IEnumerator CoroutineAStar(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();

                foreach (var item in path)
                {
                    item.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    yield return new WaitForSeconds(0.01f);
                }

                yield break;
            }

            foreach (var item in current.GetNeighbors)
            {
                float newCost = costSoFar[current] + item.Cost;
                float priority = newCost + Vector3.Distance(item.transform.position, goalNode.transform.position);

                item.GetComponent<MeshRenderer>().material.color = Color.blue;
                yield return new WaitForSeconds(0.01f);

                if (!costSoFar.ContainsKey(item))
                {
                    costSoFar.Add(item, newCost);
                    frontier.Enqueue(item, priority);
                    cameFrom.Add(item, current);
                }
                else if (costSoFar[item] > newCost)
                {
                    frontier.Enqueue(item, priority);
                    cameFrom[item] = current;
                    costSoFar[item] = newCost;
                }
            }
        }
    }

    public IEnumerator CoroutineGreedyBFS(Node startingNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();

                foreach (var item in path)
                {
                    item.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    yield return new WaitForSeconds(0.05f);
                }

                yield break;
            }

            foreach (var item in current.GetNeighbors)
            {
                
                if (!cameFrom.ContainsKey(item))
                {
                    item.GetComponent<MeshRenderer>().material.color = Color.blue;
                    yield return new WaitForSeconds(0.05f);

                    float priority = Vector3.Distance(item.transform.position, goalNode.transform.position);    
                    frontier.Enqueue(item, priority);
                    cameFrom.Add(item, current);
                }
            }
        }
        
    }

}
