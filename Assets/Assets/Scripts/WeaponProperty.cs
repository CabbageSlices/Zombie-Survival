using UnityEngine;
using System.Collections;

public class WeaponProperty : MonoBehaviour {

    public int damage = 1;
    public enum WeaponType { Gun, Melee};

    public WeaponType type = WeaponType.Gun;
}
