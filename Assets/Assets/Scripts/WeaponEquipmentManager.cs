using UnityEngine;
using System.Collections;

public class WeaponEquipmentManager : MonoBehaviour {
    
    private GameObject equippedWeapon = null;
    private WeaponProperty equippedWeaponProperty = null;
    private ObjectiveManager objectiveManager;

    private GunLogicManager gunLogicManager;
    
    void Start() {

        objectiveManager = GameObject.Find("ObjectiveManager").GetComponent<ObjectiveManager>() as ObjectiveManager;
        gunLogicManager = gameObject.GetComponent<GunLogicManager>() as GunLogicManager;

        unequipCurrentWeapon();
    }

	public void equip(GameObject weapon) {

        //if player is currently holding a weapon, drop it
        unequipCurrentWeapon();

        equippedWeapon = weapon;
        equippedWeaponProperty = weapon.GetComponent<GunProperty>();

        //put the player as a child of the weapon arm, and get rid of its physics and colliders
        weapon.transform.parent = gameObject.transform;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Rigidbody>().detectCollisions = false;
        weapon.GetComponent<Collider>().enabled = false;
        changeWeaponLayer(weapon, gameObject.layer);
        weapon.transform.localPosition = new Vector3(0, 0, 0);
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);

        objectiveManager.handleWeaponPickup(equippedWeaponProperty.type);

        gunLogicManager.enabled = true;
        gunLogicManager.equipWeapon(equippedWeapon);
    }

    void unequipCurrentWeapon() {

        //if you have a child transform then the child must be a weapon, so make it the equipped weapon
        if(transform.childCount != 0)
            equippedWeapon = transform.GetChild(0).gameObject;

        if(equippedWeapon != null) {

            equippedWeaponProperty = null;
            equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            equippedWeapon.GetComponent<Rigidbody>().detectCollisions = true;
            equippedWeapon.GetComponent<Collider>().enabled = true;
            equippedWeapon.transform.parent = null;
            changeWeaponLayer(equippedWeapon, LayerMask.NameToLayer("Interactable"));
            equippedWeapon.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        gunLogicManager.enabled = false;
    }

    //Takes a given weapon and places it onto the given player
    //useful if you are picking up a weapon on the ground, which is in the interactable layer, and equipping it, which is the weapon layer
    void changeWeaponLayer(GameObject weapon, LayerMask newLayer) {

        Transform[] objectTransforms = weapon.GetComponentsInChildren<Transform>(true);

        foreach(Transform transform in objectTransforms) {

            transform.gameObject.layer = newLayer;
        }
    }
}
