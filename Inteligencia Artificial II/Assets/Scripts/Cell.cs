using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isOccupied = false;  // Indica si la celda está ocupada
    public Vector3 position;         // La posición de la celda en el mundo

    void OnDrawGizmos()
    {
        // Dibujar un contorno de la celda en el editor para visualización
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    public bool CanPlaceTurret()
    {
        return !isOccupied;
    }

    public void RemoveTurret()
    {
        isOccupied = false;
        // Puedes añadir lógica para destruir la torreta si es necesario
    }
}

