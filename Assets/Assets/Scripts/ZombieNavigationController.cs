using UnityEngine;
using System.Collections;

public class ZombieNavigationController : MonoBehaviour {

    NavMeshAgent navMeshAgent;
    GameObject player;

    //how far the zombie needs to be from the target before it can stop moving towards it
    public float minDistanceToTarget = 5.0f;

    //how long it takes the zombie to track the player again, in seconds
    public float targetRecalulationDelay = 0.75f;
    private float lastTargetCalculationTime = 0.0f; //time that the last target was calculated

    //if an external script wants to stop navigation, set this to true
    bool isNavigationStopped = false;

    public Vector3 velocity {
        get {

            return navMeshAgent.velocity;
        }
    }

	// Use this for initialization
	void Start () {
	    
        navMeshAgent = GetComponent<NavMeshAgent>() as NavMeshAgent;
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
	    
        if(shouldRecalculateTarget()) {

            recalculateTarget();
        }
        
        if(shouldStopTranslation()) {

            navMeshAgent.velocity = new Vector3(0, 0, 0);
        }
	}

    bool shouldRecalculateTarget() {

        return Time.time - lastTargetCalculationTime > targetRecalulationDelay;
    }
    
    //attempt to track the player's current position
    //if there is a valid path to the player, it will track the player
    //if there is no valid path, it will continue along its previous path
    void recalculateTarget() {

        lastTargetCalculationTime = Time.time;

        Vector3 targetPosition = player.transform.position;

        //if zombie is close enough to player, and he is facing the player, then don't track
        Vector3 vectorToPlayer = targetPosition - transform.position;

        if(vectorToPlayer.sqrMagnitude < minDistanceToTarget * minDistanceToTarget && Vector3.Dot(transform.forward, vectorToPlayer.normalized) >= 0.9) {

            return;
        }

        NavMeshPath path = new NavMeshPath();

        if(navMeshAgent.CalculatePath(targetPosition, path)) {

            navMeshAgent.SetPath(path);
        }
    }

    public bool isNearDestination() {
        
        Vector3 vectorToTarget = navMeshAgent.pathEndPosition - transform.position;
        return vectorToTarget.sqrMagnitude < minDistanceToTarget * minDistanceToTarget && Vector3.Dot(transform.forward, vectorToTarget.normalized) >= 0.9;
    }

    bool shouldStopTranslation() {

        return isNearDestination() || isNavigationStopped;
    }

    public void stopTranslation() {

        isNavigationStopped = true;
    }

    public void resumeTranslation() {

        isNavigationStopped = false;
    }
}
