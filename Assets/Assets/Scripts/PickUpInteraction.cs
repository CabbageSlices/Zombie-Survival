using UnityEngine;
using System.Collections;
using System;

public class PickUpInteraction : InteractableAbstract {

    public override void handleInteraction(GameObject player) {

        //make the player equip this item
        player.GetComponentInChildren<WeaponManager>().equip(gameObject);
    }

    public override string getInteractionName() {

        return "Pick Up Weapon";
    }
}
