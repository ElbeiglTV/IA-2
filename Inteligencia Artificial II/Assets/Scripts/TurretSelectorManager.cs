using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretSelectorManager : MonoBehaviour
{
    private TurretData _selectedTurretData;  // La torreta seleccionada actualmente
    public TurretData SelectedTurretData { get { return _selectedTurretData; } }

    public TurretData[] turretData;  // Lista de torretas
    public TurretUIData[] turretUIData;
    public RectTransform selectionFrame; // El marco de selecci�n (RectTransform)

    private int selectedIndex = -1;  // �ndice de la torreta seleccionada

    [System.Serializable]
    public class TurretCooldown
    {
        public TurretData turretData;   // Datos del tipo de torreta
        public float lastPlacedTime;    // �ltima vez que se coloc� este tipo de torreta
    }

    public List<TurretCooldown> turretsCooldown = new List<TurretCooldown>();

    // M�todo para saber si una torreta espec�fica est� lista para ser colocada
    public bool IsReadyToPlace(TurretData turretData)
    {
        TurretCooldown turretCooldown = turretsCooldown.Find(t => t.turretData == turretData);
        if (turretCooldown != null)
        {
            return Time.time >= turretCooldown.lastPlacedTime + turretCooldown.turretData.cooldown;
        }
        return true;  // Si no encuentra el tipo de torreta, asumimos que est� lista
    }

    // M�todo para registrar la colocaci�n de una torreta
    public void RegisterPlacement(TurretData turretData)
    {
        TurretCooldown turretCooldown = turretsCooldown.Find(t => t.turretData == turretData);
        if (turretCooldown != null)
        {
            turretCooldown.lastPlacedTime = Time.time;
        }
        else
        {
            // Si es la primera vez que se coloca este tipo de torreta
            turretsCooldown.Add(new TurretCooldown { turretData = turretData, lastPlacedTime = Time.time });
        }

        // Actualiza el UI de cooldown despu�s de registrar la colocaci�n
        StartCoroutine(UpdateCooldownUI());
    }

    // M�todo para actualizar el fill de la imagen del cooldown
    public IEnumerator UpdateCooldownUI()
    {
        for (int i = 0; i < turretData.Length; i++)
        {
            TurretData data = turretData[i];
            TurretCooldown turretCooldown = turretsCooldown.Find(t => t.turretData == data);

            if (turretCooldown != null)
            {
                float cooldownProgress = Mathf.Clamp01((Time.time - turretCooldown.lastPlacedTime) / data.cooldown);
                turretUIData[i].cooldownImage.fillAmount = 1 - cooldownProgress; // El fill ser� 1 cuando est� listo
            }
            else
            {
                turretUIData[i].cooldownImage.fillAmount = 0;  // El fill est� completo si no hay cooldown
            }

            // Espera un frame despu�s de cada iteraci�n para permitir que Unity procese otras tareas.
            if (i % 5 == 0) // Por ejemplo, espera cada 5 turrets procesados.
                yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(UpdateCooldownUI());

        StartCoroutine(UpdateUI());

        // Por defecto, seleccionamos la primera torreta
        SelectTurret(0);
    }

    public void SelectTurret(int index) // M�todo para seleccionar una torreta por �ndice
    {
        if (index >= 0 && index < turretData.Length)
        {
            if (selectedIndex == index) return;  // Si ya est� seleccionada, no hacer nada

            selectedIndex = index;

            // Mover el marco de selecci�n al bot�n seleccionado
            //selectionFrame.gameObject.SetActive(true);
            selectionFrame.position = turretUIData[index].lockOverlay.transform.position;

            _selectedTurretData = turretData[index];
            Debug.Log("Torreta seleccionada: " + _selectedTurretData.turretName);
        }
    }

    // M�todo para actualizar los bloqueos seg�n la energ�a
    public void UpdateTurretAvailability(int currentEnergy)
    {
        for (int i = 0; i < turretData.Length; i++)
        {
            // Si la energ�a actual es menor al coste de la torreta, mostrar el bloqueo
            if (currentEnergy < turretData[i].energyCost)
            {
                turretUIData[i].lockOverlay.gameObject.SetActive(true);   // Mostrar el bloqueo
            }
            else
            {
                turretUIData[i].lockOverlay.gameObject.SetActive(false);  // Ocultar el bloqueo
            }
        }

        // Actualiza el UI de cooldown despu�s de registrar la colocaci�n
        StartCoroutine(UpdateCooldownUI());
    }

    public IEnumerator UpdateUI()
    {
        for (int i = 0; i < turretData.Length; i++)
        {
            // Establecer el coste de la torreta en la UI
            turretUIData[i].costText.text = turretData[i].energyCost.ToString();

            // Establecer el sprite de la torreta en la imagen
            turretUIData[i].turretImage.sprite = turretData[i].turretSprite;

            // Espera un frame despu�s de cada iteraci�n para permitir que Unity procese otras tareas.
            if (i % 5 == 0) // Por ejemplo, espera cada 5 torretas procesadas.
                yield return null;
        }
    }
}
