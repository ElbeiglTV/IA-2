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
    public RectTransform selectionFrame; // El marco de selección (RectTransform)

    private int selectedIndex = -1;  // Índice de la torreta seleccionada

    [System.Serializable]
    public class TurretCooldown
    {
        public TurretData turretData;   // Datos del tipo de torreta
        public float lastPlacedTime;    // Última vez que se colocó este tipo de torreta
    }

    public List<TurretCooldown> turretsCooldown = new List<TurretCooldown>();

    // Método para saber si una torreta específica está lista para ser colocada
    public bool IsReadyToPlace(TurretData turretData)
    {
        TurretCooldown turretCooldown = turretsCooldown.Find(t => t.turretData == turretData);
        if (turretCooldown != null)
        {
            return Time.time >= turretCooldown.lastPlacedTime + turretCooldown.turretData.cooldown;
        }
        return true;  // Si no encuentra el tipo de torreta, asumimos que está lista
    }

    // Método para registrar la colocación de una torreta
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

        // Actualiza el UI de cooldown después de registrar la colocación
        StartCoroutine(UpdateCooldownUI());
    }

    // Método para actualizar el fill de la imagen del cooldown
    public IEnumerator UpdateCooldownUI()
    {
        for (int i = 0; i < turretData.Length; i++)
        {
            TurretData data = turretData[i];
            TurretCooldown turretCooldown = turretsCooldown.Find(t => t.turretData == data);

            if (turretCooldown != null)
            {
                float cooldownProgress = Mathf.Clamp01((Time.time - turretCooldown.lastPlacedTime) / data.cooldown);
                turretUIData[i].cooldownImage.fillAmount = 1 - cooldownProgress; // El fill será 1 cuando esté listo
            }
            else
            {
                turretUIData[i].cooldownImage.fillAmount = 0;  // El fill está completo si no hay cooldown
            }

            // Espera un frame después de cada iteración para permitir que Unity procese otras tareas.
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

    public void SelectTurret(int index) // Método para seleccionar una torreta por índice
    {
        if (index >= 0 && index < turretData.Length)
        {
            if (selectedIndex == index) return;  // Si ya está seleccionada, no hacer nada

            selectedIndex = index;

            // Mover el marco de selección al botón seleccionado
            //selectionFrame.gameObject.SetActive(true);
            selectionFrame.position = turretUIData[index].lockOverlay.transform.position;

            _selectedTurretData = turretData[index];
            Debug.Log("Torreta seleccionada: " + _selectedTurretData.turretName);
        }
    }

    // Método para actualizar los bloqueos según la energía
    public void UpdateTurretAvailability(int currentEnergy)
    {
        for (int i = 0; i < turretData.Length; i++)
        {
            // Si la energía actual es menor al coste de la torreta, mostrar el bloqueo
            if (currentEnergy < turretData[i].energyCost)
            {
                turretUIData[i].lockOverlay.gameObject.SetActive(true);   // Mostrar el bloqueo
            }
            else
            {
                turretUIData[i].lockOverlay.gameObject.SetActive(false);  // Ocultar el bloqueo
            }
        }

        // Actualiza el UI de cooldown después de registrar la colocación
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

            // Espera un frame después de cada iteración para permitir que Unity procese otras tareas.
            if (i % 5 == 0) // Por ejemplo, espera cada 5 torretas procesadas.
                yield return null;
        }
    }
}
