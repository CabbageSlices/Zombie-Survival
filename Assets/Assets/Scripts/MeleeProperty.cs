using UnityEngine;
using System.Collections;

public class MeleeProperty : WeaponProperty {

    public MeleeProperty() {

        damage = 1;
        type = WeaponProperty.WeaponType.Melee;
    }
}