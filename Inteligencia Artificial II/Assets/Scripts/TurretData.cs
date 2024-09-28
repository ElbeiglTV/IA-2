using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTurretData", menuName = "Turret/TurretData")]
public class TurretData : ScriptableObject
{
    public string turretName;
    public int energyCost;
    public float cooldown;
    public GameObject turretPrefab;
    public Sprite turretSprite;  // Añadimos el sprite de la torreta
}
