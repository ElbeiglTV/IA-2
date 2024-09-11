using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public TextMeshProUGUI textCost;
    public List<Node> neighbors = new List<Node>();
    public Collider nodeCollider => GetComponent<Collider>();
    int _cost;

    public int Cost { get { return _cost; } }

    public List<Node> GetNeighbors
    {
        get 
        {
            return neighbors;
        }
    }
    private void Start()
    {
        GameManager.Instance.grid.grid.Add(this);
    }



    private void OnDrawGizmos()
    {
        foreach (var neighbor in neighbors)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }




    private void OnMouseOver()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _cost++;
            if (_cost > 50)
                _cost = 50;
            textCost.text = _cost.ToString();
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            _cost--;
            if (_cost < 1)
                _cost = 1;

            textCost.text = _cost.ToString();
        }

    }
}
