using UnityEngine;
using System.Collections;

public interface IinteractionHandler {

	void handleInteraction(GameObject player);
    string getInteractionName();
}
