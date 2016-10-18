using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    //response to a collision is carried out via events
    //collisionFunctionCallerObject is the object who triggered the collision event
    //otherObjectCollider is the collider of the other object that the caller object collided with
    public delegate void OnHit(GameObject collisionFunctionCallerObject, Collider otherObjectCollider);
    public event OnHit onHit;

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Zombie") {

            //player jsut got hit by a zombie
            other.enabled = false;
            GetComponent<HealthManager>().getHit(2);

            if (onHit != null)
                onHit(gameObject, other);
        }
    }
}
