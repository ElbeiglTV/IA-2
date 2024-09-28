using FriendlyEditor.UtilityAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [StringPopup(new[] {"Pescado azul", "Pescado rojo", "Pescado verde", 
                        "Espada", "Escudo", "Hacha", 
                        "Tarta", "Torta", "Pan", "Galletas",
                        "Banana", "Zanahoria", "Sandía", 
                        "Carne", "Jamón", "Chorizo" })]
    public string itemType;

    public Stand stand;
    public int price; // minorist price

    public int cost; // cost to buy

    public bool active; // is the item active

    private void Start()
    {
       stand.standObjects.Add(gameObject);
    }

}
[System.Serializable]
public class NMItem
{
    [StringPopup(new[] {"Pescado azul", "Pescado rojo", "Pescado verde",
                        "Espada", "Escudo", "Hacha",
                        "Tarta", "Torta", "Pan", "Galletas",
                        "Banana", "Zanahoria", "Sandía",
                        "Carne", "Jamón", "Chorizo" })]
    public string itemType;

}
