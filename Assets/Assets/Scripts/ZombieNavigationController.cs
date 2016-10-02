using UnityEngine;
using System.Collections;

public class ZombieNavigationController : MonoBehaviour {

    NavMeshAgent navMeshAgent;
    GameObject player;

    //how far the zombie needs to be from the target before it can stop moving towards it
    public float minDistanceToTarget = 8.0f;

    //how long it takes the zombie to track the player again, in seconds
    public float targetRecalulationDelay = 0.75f;
    private float lastTargetCalculationTime = 0.0f; //time that the last target was calculated

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
        
        if(isNearDestination()) {

            navMeshAgent.Stop();
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

        //if zombie is close enough to player then don't track 
        Vector3 vectorToPlayer = targetPosition - transform.position;

        if(vectorToPlayer.sqrMagnitude < minDistanceToTarget * minDistanceToTarget ) {

            return;
        }

        NavMeshPath path = new NavMeshPath();

        if(navMeshAgent.CalculatePath(targetPosition, path)) {

            navMeshAgent.SetPath(path);
            navMeshAgent.Resume();
        }
    }

    bool isNearDestination() {
        
        Vector3 vectorToTarget = navMeshAgent.pathEndPosition - transform.position;
        return vectorToTarget.sqrMagnitude < minDistanceToTarget * minDistanceToTarget;
    }
}
