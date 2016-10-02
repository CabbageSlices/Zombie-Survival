using UnityEngine;
using System.Collections;

public class InteractWithObjects : MonoBehaviour {
	
    public float MaxInteractionDistace = 10;
    public LayerMask layerIdOfInteractable = -1;

    //delay between each time the game checks if the player is looking at an interactable object
    public float objectCheckDelay = 0.4f;

    //the transform of the player's main camera which act as the eyes of the player, used to get the player's view direction for raycasting
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
            (interactableTarget.GetComponent<InteractableAbstract>() as InteractableAbstract).handleInteraction(gameObject); 

	}

    //check if the player is looking at an interactable object
    IEnumerator CheckViewingObject() {

        //always check for an interactable object in the players view
        while(true) {

            Vector3 rayOrigin = playerCameraTransform.position;
            Vector3 rayDirection = playerCameraTransform.TransformDirection(Vector3.forward);
            RaycastHit target;

            interactableTarget = null;

            Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * 10, Color.red, objectCheckDelay);

            if(Physics.Raycast(rayOrigin, rayDirection, out target, MaxInteractionDistace, layerIdOfInteractable)) {

                //the desired target could be the object hit by the raycast, or it could be the parent of the object hit by the raycast
                //desired object will have an InteractableAbstract script component attatched 
                interactableTarget = target.transform.gameObject.GetComponent<InteractableAbstract>() == null ? target.transform.parent.gameObject : target.transform.gameObject;

                //if even the parent game object doesn't have an interactable abstract object, something went wrong
                if(interactableTarget.GetComponent<InteractableAbstract>() != null) {

                    string interactionName = (interactableTarget.GetComponent<InteractableAbstract>() as InteractableAbstract).getInteractionName();
                    screenUIManager.setInteractionMessage(string.Format("Press E To {0}", interactionName));

                } else {

                    interactableTarget = null;
                }
                

            } else {

                screenUIManager.setInteractionMessage("");
            }

            yield return new WaitForSeconds(objectCheckDelay);
        }
    }
}
