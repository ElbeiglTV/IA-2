using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct TurretUIData
{
    public TextMeshProUGUI costText;
    public Image cooldownImage;
    public Image lockOverlay;   // Imagen que bloquea si no hay suficiente energ�a
    public Image turretImage;   // Nueva imagen para el sprite de la torreta
}
