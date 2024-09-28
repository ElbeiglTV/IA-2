using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlacementManager : MonoBehaviour
{
    public EnergySystem energySystem; // Sistema de energía
    public TurretSelectorManager turretSelector;

    void Update()
    {
        // Detección de click izquierdo para colocar torretas
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Cell selectedCell = hit.collider.GetComponent<Cell>();
                if (selectedCell != null && selectedCell.CanPlaceTurret())
                {
                    PlaceTurret(selectedCell);
                }
                else
                {
                    Debug.Log("No se puede colocar la torreta aquí.");
                }
            }
        }
    }

    void PlaceTurret(Cell cell)
    {
        TurretData selectedTurretData = turretSelector.SelectedTurretData;  // Obtener los datos de la torreta seleccionada

        if (!turretSelector.IsReadyToPlace(selectedTurretData))
        {
            Debug.Log("Torreta en cooldown.");
            return;
        }

        if (energySystem.CanPlaceTurret(selectedTurretData.energyCost))
        {
            // Instanciar la torreta en la posición de la celda
            GameObject placedTurret = Instantiate(selectedTurretData.turretPrefab, cell.position, Quaternion.identity);

            // Registrar la colocación de la torreta
            turretSelector.RegisterPlacement(selectedTurretData);

            // Asignar la celda y gestionar el estado de la torreta colocada
            placedTurret.GetComponent<Turret>().SetCell(cell);
            cell.isOccupied = true;

            // Restar energía
            energySystem.UseEnergy(selectedTurretData.energyCost);
        }
        else
        {
            Debug.Log("No hay suficiente energía.");
        }
    }


}

