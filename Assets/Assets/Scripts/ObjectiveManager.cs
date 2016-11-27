using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectiveManager : MonoBehaviour {

    //list of all the objectives the player is currently doing
	List<Objective> objectivesInProgress = new List<Objective>();

    List<Objective> objectivesCompleted = new List<Objective>();

    //sort objectives by their completion conditions into separate lists, that way whenever an event is triggered
    //that could potentially complete an objective, the manager can look through a list where the event is relevant
    //instead of having to sort through every single objective
    List<Objective> equipWeaponObjectives = new List<Objective>();
    
    public delegate void OnObjectiveEvent(Objective objective);
    
    public event OnObjectiveEvent onObjectiveStart;
    public event OnObjectiveEvent onObjectiveFinish;

    void Start() {

        Debug.Log("asdf");
        startObjective(1);
    }

    //create the objective with the given id
    public void startObjective(int objectiveId) {

        Objective objective = new Objective(objectiveId);
        objectivesInProgress.Add(objective);
        Debug.Log("Started Objective " + objective.name);

        if(objective.type == Objective.ObjectiveType.EquipWeapon)
            equipWeaponObjectives.Add(objective);

        if(onObjectiveStart != null)
            onObjectiveStart(objective);
    }

    public void finishObjective(Objective objective) {

        objectivesInProgress.RemoveAll(obj => obj.objectiveId == objective.objectiveId );
        Debug.Log("Finished Objective " + objective.name);

        objectivesCompleted.Add(objective);

        if (objective.type == Objective.ObjectiveType.EquipWeapon)
            equipWeaponObjectives.RemoveAll(obj => obj.objectiveId == objective.objectiveId);

        if (onObjectiveFinish != null)
            onObjectiveFinish(objective);
    }

    public void handleWeaponPickup(WeaponProperty.WeaponType weaponType) {
        
        for(int i = equipWeaponObjectives.Count - 1; i >= 0; --i) {

            if(equipWeaponObjectives[i].completionCondition.requiredWeaponType == weaponType)
                finishObjective(equipWeaponObjectives[i]);
        }
    }
}
