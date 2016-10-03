using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ZombieNavigationController))]
public class ZombieController : MonoBehaviour {

    private ZombieNavigationController navigationController;
    private Animator animator;

    private int speedHash = Animator.StringToHash("Speed");
    private int attackTriggerHash = Animator.StringToHash("Attack");

    //how much time the zombie must wait before attacking again, in seconds
    [SerializeField]
    private float attackDelay = 3f;

    //last time the zombie attacked
    float lastAttackTime = 0;

	// Use this for initialization
	void Start () {
	    
        animator = GetComponent<Animator>() as Animator;
        navigationController = GetComponent<ZombieNavigationController>() as ZombieNavigationController;

        if(navigationController == null)
            Debug.LogError(gameObject.name + " does not have a navigation controller.");

        if(animator == null)
            Debug.LogError(gameObject.name + " does not have an animation controller, even though it has an animation controller script.");
	}
	
	// Update is called once per frame
	void Update () {
	    
        animator.SetFloat(speedHash, navigationController.velocity.magnitude);

        if(navigationController.isNearDestination() && canAttack()) {

            attack();
        }
	}

    bool canAttack() {

        return Time.time - lastAttackTime > attackDelay;
    }

    void attack() {

        animator.SetTrigger(attackTriggerHash);
        lastAttackTime = Time.time;
        navigationController.stopNavigation();
    }
}
