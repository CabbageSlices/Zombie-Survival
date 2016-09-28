﻿using UnityEngine;
using System.Collections;

public class InteractWithObjects : MonoBehaviour {
	
    public float MaxInteractionDistace = 10;
    public LayerMask layerIdOfInteractable = -1;

    //delay between each time the game checks if the player is looking at an interactable object
    public float objectCheckDelay = 0.4f;

    private Transform playerCameraTransform;
    private ScreenUIManager screenUIManager;

    //current target that can be interacted with
    private GameObject interactableTarget { get; set;}

    void Start() {

        screenUIManager = GameObject.Find("ScreenUI").GetComponent<ScreenUIManager>() as ScreenUIManager;
        playerCameraTransform = (GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren(typeof(Camera)) as Camera).transform;
        StartCoroutine(CheckViewingObject());
    }

	// Update is called once per frame
	void Update () {
	    
        //check if user found an object he can interact with
        if(Input.GetKeyDown(KeyCode.E) && interactableTarget != null)
            //Debug.Log("You Used" + interactableTarget.name);
            (interactableTarget.GetComponent<InteractableAbstract>() as IinteractionHandler).handleInteraction(gameObject); 

	}

    //check if the player is looking at an interactable object
    IEnumerator CheckViewingObject() {

        while(true) {

            Vector3 rayOrigin = playerCameraTransform.position;
            Vector3 rayDirection = playerCameraTransform.TransformDirection(Vector3.forward);
            RaycastHit target;

            interactableTarget = null;
            if(Physics.Raycast(rayOrigin, rayDirection, out target, MaxInteractionDistace, layerIdOfInteractable)) {

                //the desired target could be the object hit by the raycast, or it could be the parent of the object hit by the raycast
                //desired object will have an InteractableAbstract script component attatched 
                interactableTarget = target.transform.gameObject.GetComponent<InteractableAbstract>() == null ? target.transform.parent.gameObject : target.transform.gameObject;
                string interactionName = (interactableTarget.GetComponent<InteractableAbstract>() as InteractableAbstract).getInteractionName();

                screenUIManager.setInteractionMessage(string.Format("Press E To {0}", interactionName) );

            } else {

                screenUIManager.setInteractionMessage("");
            }

            yield return new WaitForSeconds(objectCheckDelay);
        }
    }
}
