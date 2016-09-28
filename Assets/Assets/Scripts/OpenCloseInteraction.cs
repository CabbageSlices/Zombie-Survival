using UnityEngine;
using System.Collections;
using System;

public class OpenCloseInteraction : InteractableAbstract {

    public bool isOpen = false;

    private Animator anim;
    private int isOpenHash = Animator.StringToHash("isOpen");

	// Use this for initialization
	void Start () {
	    
        anim = GetComponent<Animator>();
	}
	
	public override void handleInteraction(GameObject player) {

        isOpen = !isOpen;
        anim.SetBool(isOpenHash, isOpen);
    }

    public override string getInteractionName() {
        
        return isOpen ? "Close" : "Open";
    }
}
