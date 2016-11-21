using UnityEngine;
using System.Collections;

public class Objective {

    //objective completion conditions, even thoguh a single obejctive object contains information about completion
    //conditions for all types of objectives, the only condition that will be used is the one relating to the obejctives type
    //ex: only objectives of type EquipWeapon will use the requiredWeapon field, otherwise it will be left blank
    public class ObjectiveCompletionCondition {

        public GunProperty.WeaponType requiredWeaponType;
    }

    //type of objective determines which event causes the objective to progress
    //equipWeapon - objective is completed once user equips the required weapon
    public enum ObjectiveType { EquipWeapon };
    
    public string name;
    public string description;

    public int objectiveId;

    public ObjectiveType type = ObjectiveType.EquipWeapon;
    public ObjectiveCompletionCondition completionCondition;

    public Objective(int id) {

        objectiveId = id;

        name = "Pick up the pistol";
        type = ObjectiveType.EquipWeapon;
        completionCondition = new ObjectiveCompletionCondition();
        completionCondition.requiredWeaponType = GunProperty.WeaponType.Gun;
    }
}
