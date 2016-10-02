using UnityEngine;
using System.Collections;

public class ZombieNavigationController : MonoBehaviour {

    NavMeshAgent navMeshAgent;
	// Use this for initialization
	void Start () {
	    
        navMeshAgent = GetComponent<NavMeshAgent>() as NavMeshAgent;
    }
	
	// Update is called once per frame
	void Update () {
	    
        GetComponent<NavMeshAgent>().destination = GameObject.FindGameObjectWithTag("Player").transform.position;
	}
}
