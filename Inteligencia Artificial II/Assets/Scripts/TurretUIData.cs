using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct TurretUIData
{
    public TextMeshProUGUI costText;
    public Image cooldownImage;
    public Image lockOverlay;   // Imagen que bloquea si no hay suficiente energía
    public Image turretImage;   // Nueva imagen para el sprite de la torreta
}
