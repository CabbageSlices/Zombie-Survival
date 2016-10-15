using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthOverlayController : MonoBehaviour {

    [SerializeField]
    private HealthManager playerHealthManager;

    [SerializeField]
    private Image damageOverlayImage;

    [SerializeField]
    private Image screenFlashOverlayImage;

    private bool isScreenFlashing = false;

    //how long the screen should flash, in seconds
    float screenFlashLength = 0.5f;
    float timeScreenStartedFlashing = 0.0f;

    public float screenFlashSpeed = 20;

	// Use this for initialization
	void Start () {

        if(damageOverlayImage == null)
            Debug.Log("This object is missing the image component in the child DamageOverlayImage");

        if(screenFlashOverlayImage == null)
            Debug.Log("This object is missing the image component in the child ScreenFlashOverlayImage");

        if(playerHealthManager == null)
            Debug.Log("The object is missing player's health manager.");
        
        subscribeToEvents();

        setImageTransparency(damageOverlayImage, 0);
        setImageTransparency(screenFlashOverlayImage, 0);
    }

    void Update() { 

        Vector4 newColor = screenFlashOverlayImage.color;

        if (isScreenFlashing) {

            newColor.w = Mathf.Sin(screenFlashSpeed * (Time.time - timeScreenStartedFlashing) );
            
            //normalize the color into the range [0, 1]
            newColor.w = (newColor.w + 1) / 2;

            //stop flashing screen if its been flashing for long enough
            isScreenFlashing = Time.time < (timeScreenStartedFlashing + screenFlashLength);

        } else {

            //screens not flashing so get rid of lash effect completely
            newColor.w = 0;
        }

        screenFlashOverlayImage.color = newColor;
    }

    void OnEnable() {

        subscribeToEvents();
    }

    void OnDisable() {

        unsubscribeFromEvents();
    }

    void subscribeToEvents() {
        
        playerHealthManager.onHit += onHit;
    }

    void unsubscribeFromEvents() {

        playerHealthManager.onHit -= onHit;
    }
	
	void onHit(int damageReceived, float fractionHealthRemaining) {

        //player just got hit, start by setting the desired transparency of the overlay
        //when players healht is 0, effect should be fully visable, when player has 100% health, it should be invisible
        setImageTransparency(damageOverlayImage, Mathf.Clamp(1 - fractionHealthRemaining, 0, 1));

        //now we want to try applying a flashing effect, only if the screen isn't flashing already
        if(!isScreenFlashing)
            startScreenFlashEffect();
    }

    void setImageTransparency(Image image, float transparency) {

        Vector4 newColor = image.color;
        newColor.w = transparency;
        image.color = newColor;
    }

    void startScreenFlashEffect() {

        isScreenFlashing = true;
        timeScreenStartedFlashing = Time.time;
    }
}
