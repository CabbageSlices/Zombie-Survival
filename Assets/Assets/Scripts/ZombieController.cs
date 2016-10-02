using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ZombieNavigationController))]
public class ZombieController : MonoBehaviour {

    private ZombieNavigationController navigationController;
    private Animator modelAnimator;

	// Use this for initialization
	void Start () {
	    
        modelAnimator = GetComponentInChildren<Animator>() as Animator;
        navigationController = GetComponent<ZombieNavigationController>() as ZombieNavigationController;

        if(navigationController == null)
            Debug.LogError(gameObject.name + " does not have a navigation controller.");

        if(modelAnimator == null)
            Debug.LogError(gameObject.name + " does not have an animation controller, even though it has an animation controller script.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
