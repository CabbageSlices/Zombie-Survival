using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenUIManager : MonoBehaviour {

    Text interactionMessage;
    
	// Use this for initialization
	void Start () {
	    
        interactionMessage = transform.Find("InteractionMessage").GetComponent<Text>() as Text;
	}
	
	// Update is called once per frame
	public void setInteractionMessage(string newMessage) {
        
        interactionMessage.text = newMessage;
    }
}
