using UnityEngine;
using System.Collections;

public class GunProperty : WeaponProperty {
    
    public float fireDelay = 1;
    public float range = 100.0f;
    public int bulletsPerMagazine = 5;
    public int bulletsInCurrentMagazine = 5;
    public int remainingBullets = 25;

    public GunProperty() {

        damage = 3;
        type = WeaponProperty.WeaponType.Gun;
    }
}
