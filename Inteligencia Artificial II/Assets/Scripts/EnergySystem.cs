using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    public TextMeshProUGUI energyText;  // Para TextMeshPro

    public TurretSelectorManager turretSelector;
    public int currentEnergy = 100;
    public int energyPerSecond = 5;

    void Start()
    {
        // Generar energía cada segundo
        InvokeRepeating("GenerateEnergy", 1f, 1f);
    }

    void GenerateEnergy()
    {
        currentEnergy += energyPerSecond;

        UpdateUI();

        //Debug.Log("Energía actual: " + currentEnergy);
    }

    public bool CanPlaceTurret(int cost)
    {
        return currentEnergy >= cost;
    }

    public void UseEnergy(int cost)
    {
        currentEnergy -= cost;

        UpdateUI();
    }

    private void UpdateUI()
    {
        energyText.text = /*"Energy: " +*/ currentEnergy.ToString();

        turretSelector.UpdateTurretAvailability(currentEnergy);
    }
}

