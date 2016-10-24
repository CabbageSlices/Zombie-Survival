using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

    private GameObject weaponOwner;
    private Camera playerCamera;
    private GameObject equippedWeapon = null;
    private GunProperty equippedWeaponProperty = null;
    private Animator weaponAnimator;
    private ScreenUIManager screenUIManager;
    
    private int isAimingDownSightsHash = Animator.StringToHash("IsAimingDownSights");
    private int firedTriggerHash = Animator.StringToHash("Fired");

    private float lastFiredTime = 0;

    private float reloadStartTime = -3;

    enum State {Idle /*idle is when gun is at hip, and while aiming down sights*/, Reloading };
    State currentState;
    
    void Start() {

        screenUIManager = GameObject.Find("ScreenUI").GetComponent<ScreenUIManager>() as ScreenUIManager;
        weaponAnimator = gameObject.GetComponent<Animator>();

        //find the game object that is holding this weapon. This will the highest object in the transform heirchy
        weaponOwner = gameObject.transform.root.gameObject;
        playerCamera = weaponOwner.GetComponentInChildren<Camera>() as Camera;
        
        subscribeToEvents();

        unequipCurrentWeapon();
    }

    void OnEnable() {

        subscribeToEvents();    
    }

    void OnDisable() {

        unsubscribeFromEvents();
    }

    void enterState(State toEnter) {

        if(toEnter == State.Reloading) {

            reloadStartTime = Time.time;
        }

        currentState = toEnter;
    }

    void exitState() {

        if(currentState == State.Reloading) {

            reloadStartTime = -3;
            screenUIManager.setAmmoDisplay(equippedWeaponProperty.bulletsInCurrentMagazine, equippedWeaponProperty.remainingBullets);
        }
    }

    void changeState(State newState) {

        exitState();
        enterState(newState);
    }

    void subscribeToEvents() {

        if (weaponOwner != null) {

            weaponOwner.GetComponent<InputManager>().onWeaponPrimary += onWeaponUse;
            weaponOwner.GetComponent<InputManager>().isPressingAim += checkAimDownSight;
        }
    }

    void unsubscribeFromEvents() {

        if (weaponOwner != null) {

            weaponOwner.GetComponent<InputManager>().onWeaponPrimary += onWeaponUse;
            weaponOwner.GetComponent<InputManager>().isPressingAim -= checkAimDownSight;
        }
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

        screenUIManager.showAmmoDisplay(equippedWeaponProperty.bulletsInCurrentMagazine, equippedWeaponProperty.remainingBullets);
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

        screenUIManager.hideAmmoDisplay();
    }

    //Takes a given weapon and places it onto the given player
    //useful if you are picking up a weapon on the ground, which is in the interactable layer, and equipping it, which is the weapon layer
    void changeWeaponLayer(GameObject weapon, LayerMask newLayer) {

        Transform[] objectTransforms = weapon.GetComponentsInChildren<Transform>(true);

        foreach(Transform transform in objectTransforms) {

            transform.gameObject.layer = newLayer;
        }
    }

    void Update() {

        if(currentState == State.Reloading) {

            if(Time.time - reloadStartTime >= 2.5f) {

                reload();
                changeState(State.Idle);
            }

        } else if(currentState == State.Idle) {

            if(shouldForceReload())
                changeState(State.Reloading);
        }
    }

    public void onWeaponUse() {

        if (checkCanFire()) {

            if(currentState == State.Reloading)
                changeState(State.Idle);

            fire();
        } 
    }

    bool checkCanFire() {

        if(equippedWeapon == null || equippedWeaponProperty == null)
            return false;

        if(Time.time - lastFiredTime < equippedWeaponProperty.fireDelay)
            return false;

        if(equippedWeaponProperty.bulletsInCurrentMagazine == 0)
            return false;

        return true;
    }

    bool shouldForceReload() {

        if (equippedWeapon == null || equippedWeaponProperty == null)
            return false;

        if(equippedWeaponProperty.bulletsInCurrentMagazine > 0)
            return false;

        if(equippedWeaponProperty.remainingBullets == 0)
            return false;

        return true;
    }

    void reload() {
        
        int bulletsRequiredForFullMagazine = equippedWeaponProperty.bulletsPerMagazine - equippedWeaponProperty.bulletsInCurrentMagazine;
        int refilledBullets = Mathf.Min(bulletsRequiredForFullMagazine, equippedWeaponProperty.remainingBullets);

        equippedWeaponProperty.bulletsInCurrentMagazine += refilledBullets;
        equippedWeaponProperty.remainingBullets -= refilledBullets;
    }

    void fire() {
       
        weaponAnimator.SetTrigger(firedTriggerHash);
        lastFiredTime = Time.time;

        //handle ammo
        equippedWeaponProperty.bulletsInCurrentMagazine -= 1;
        screenUIManager.setAmmoDisplay(equippedWeaponProperty.bulletsInCurrentMagazine, equippedWeaponProperty.remainingBullets);

        //fire a raycast from player camera's center in the direction of the camera
        Vector3 raycastOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 raycastDirection = playerCamera.transform.TransformDirection(Vector3.forward);
        RaycastHit targetHit;

        if(Physics.Raycast(raycastOrigin, raycastDirection, out targetHit, equippedWeaponProperty.range)) {

            //if the object has a healthmanager then damage it
            Component healthManagerComponent = targetHit.transform.gameObject.GetComponent<HealthManager>();

            //no health manager, don't do anything to this object
            if(healthManagerComponent == null)
                return;

            HealthManager healthManager = (HealthManager)healthManagerComponent;
            healthManager.getHit(equippedWeaponProperty.damage);
        }
    }

    void checkAimDownSight(bool isAimingDownSight) {

        if(currentState == State.Reloading) {

            isAimingDownSight = shouldForceReload() ? false : isAimingDownSight;

            if(isAimingDownSight)
                changeState(State.Idle);
        }

        weaponAnimator.SetBool(isAimingDownSightsHash, isAimingDownSight);
    }
}
