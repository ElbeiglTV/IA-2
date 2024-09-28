using FriendlyEditor.UtilityAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [StringPopup(new[] {"Pescado","Arma","Prosesado","Vegetales","Carnes" })]
    public string itemType;

    public int price; // minorist price
    public int cost; // cost to buy

    public bool active; // is the item active

}
