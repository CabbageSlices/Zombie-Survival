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
    private int startReloadingHash = Animator.StringToHash("StartReloading");
    private int stopReloadingHash = Animator.StringToHash("StopReloading");
    private int isSprintingHash = Animator.StringToHash("IsSprinting");

    private float lastFiredTime = 0;

    enum State {Idle /*idle is when gun is at hip, and while aiming down sights*/, Reloading, Sprinting };
    State currentState = State.Idle;
    
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
            
            weaponAnimator.SetTrigger(startReloadingHash);
            weaponAnimator.ResetTrigger(stopReloadingHash);

        } else if(toEnter == State.Sprinting) {

            weaponAnimator.SetBool(isSprintingHash, true);
        }

        currentState = toEnter;
    }

    void exitState() {

        if(currentState == State.Reloading) {
            
            screenUIManager.setAmmoDisplay(equippedWeaponProperty.bulletsInCurrentMagazine, equippedWeaponProperty.remainingBullets);
            weaponAnimator.SetTrigger(stopReloadingHash);
            weaponAnimator.ResetTrigger(startReloadingHash);

        } else if(currentState == State.Sprinting){

            weaponAnimator.SetBool(isSprintingHash, false);
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
            weaponOwner.GetComponent<InputManager>().onReload += onReload;
            weaponOwner.GetComponent<InputManager>().onAimDownSight += onAimDownSight;
            weaponOwner.GetComponent<InputManager>().isSprinting += checkSprinting;
        }
    }

    void unsubscribeFromEvents() {

        if (weaponOwner != null) {

            weaponOwner.GetComponent<InputManager>().onWeaponPrimary += onWeaponUse;
            weaponOwner.GetComponent<InputManager>().isPressingAim -= checkAimDownSight;
            weaponOwner.GetComponent<InputManager>().onReload -= onReload;
            weaponOwner.GetComponent<InputManager>().onAimDownSight -= onAimDownSight;
            weaponOwner.GetComponent<InputManager>().isSprinting -= checkSprinting;
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

        if(currentState == State.Idle) {

            if(shouldForceReload())
                changeState(State.Reloading);
        }
    }

    public void onWeaponUse() {

        if (currentState == State.Idle && checkCanFire()) {
            
            fire();
        } 
    }

    public void onReload() {

        if(currentState != State.Reloading && checkCanReload())
            changeState(State.Reloading);
    }

    bool isWeaponEquipped() {

        return equippedWeapon != null && equippedWeaponProperty != null;
    }

    bool checkCanFire() {

        if(!isWeaponEquipped())
            return false;

        if(Time.time - lastFiredTime < equippedWeaponProperty.fireDelay)
            return false;

        if(equippedWeaponProperty.bulletsInCurrentMagazine == 0)
            return false;

        return true;
    }

    bool checkCanReload() {

        if(!isWeaponEquipped())
            return false;

        if(equippedWeaponProperty.bulletsInCurrentMagazine >= equippedWeaponProperty.bulletsPerMagazine)
            return false;

        if(equippedWeaponProperty.remainingBullets == 0)
            return false;

        return true;
    }

    bool shouldForceReload() {

        return checkCanReload() && equippedWeaponProperty.bulletsInCurrentMagazine == 0;
    }

    public void reload() {
        
        int bulletsRequiredForFullMagazine = equippedWeaponProperty.bulletsPerMagazine - equippedWeaponProperty.bulletsInCurrentMagazine;
        int refilledBullets = Mathf.Min(bulletsRequiredForFullMagazine, equippedWeaponProperty.remainingBullets);

        equippedWeaponProperty.bulletsInCurrentMagazine += refilledBullets;
        equippedWeaponProperty.remainingBullets -= refilledBullets;
        changeState(State.Idle);
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

    //two events to check if useri s aiming down sights
    //this function puts user in the aim donw sights state as long as he isn't reloading
    //this way a user can hold the aim down sights key, reload, and return to aiming down sights without having to
    //press the button again.
    void checkAimDownSight(bool isAimingDownSight) {

        isAimingDownSight = isAimingDownSight && currentState == State.Idle;

        weaponAnimator.SetBool(isAimingDownSightsHash, isAimingDownSight);
    }

    //all this should do is, if user is reloading then stop reloading and go back to aiming down sights
    void onAimDownSight() {

        if (currentState == State.Reloading && !shouldForceReload()) {

            changeState(State.Idle);
        }
    }

    void checkSprinting(bool isSprinting) {

        if(isSprinting && currentState != State.Sprinting) {

            changeState(State.Sprinting);
        }

        if(!isSprinting && currentState == State.Sprinting) {

            changeState(State.Idle);
        }
    }
}
