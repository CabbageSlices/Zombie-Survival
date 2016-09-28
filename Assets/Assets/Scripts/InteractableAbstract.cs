using UnityEngine;
using System.Collections;

public abstract class InteractableAbstract : MonoBehaviour, IinteractionHandler {

	public abstract void handleInteraction(GameObject player);
    public abstract string getInteractionName();
}
