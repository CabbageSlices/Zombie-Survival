using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenUIManager : MonoBehaviour {

    Text interactionMessage;
    Text ammoDisplayText;
    
	// Use this for initialization
	void Start () {
	    
        interactionMessage = transform.Find("InteractionMessage").GetComponent<Text>() as Text;
        ammoDisplayText = transform.Find("AmmoDisplay").GetComponent<Text>() as Text;

        if(interactionMessage == null)
            Debug.LogWarning("Screen UI has no interaction message.");

        if(ammoDisplayText == null)
            Debug.LogWarning("Screen UI has no ammo display.");
	}
	
	public void setInteractionMessage(string newMessage) {
        
        interactionMessage.text = newMessage;
    }
    
    public void setAmmoDisplay(int ammoInCurrentMagazine, int totalRemainingAmmo) {

        ammoDisplayText.text = ammoInCurrentMagazine.ToString() + "/" + totalRemainingAmmo.ToString();
    }

    public void hideAmmoDisplay() {

        ammoDisplayText.enabled = false;
    }

    public void showAmmoDisplay(int ammoInCurrentMagazine, int totalRemainingAmmo) {

        ammoDisplayText.enabled = true;
        setAmmoDisplay(ammoInCurrentMagazine, totalRemainingAmmo);
    }
}
