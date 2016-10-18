using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Images {

    public Image damageOverlayImage;
    public Image screenFlashOverlayImage;
    public Image attackerLocationIndicatorImage;
}

public class DamageIndicatorUIController : MonoBehaviour {

    [SerializeField]
    private HealthManager playerHealthManager;

    [SerializeField]
    GameObject player;

    [SerializeField]
    PlayerController playerController;

    //the last attacker that attacked the zombie
    //used to recalculate the rotation of the attacker position indicator
    private GameObject lastAttacker;

    public Images images = new Images();

    //how long the screen should flash, in seconds
    float screenFlashLength = 0.5f;
    float timeScreenStartedFlashing = -10;

    public float screenFlashSpeed = 20;

    //how long the attacker's location indicator should be drawn, in seconds
    [SerializeField] float attackerLocationIndicatorDisplayLength = 0.5f;
    float timeAttackerLocationIndicatorStartedDrawing = -10;

    void Start() {

        player = GameObject.FindWithTag("Player");

        if (player == null)
            Debug.Log("Player not found");

        playerController = player.GetComponent<PlayerController>() as PlayerController;

        if(playerController == null)
            Debug.Log("Player has no player controller script");

        if (images.damageOverlayImage == null)
            Debug.Log("This object is missing the image component in the child DamageOverlayImage");

        if (images.screenFlashOverlayImage == null)
            Debug.Log("This object is missing the image component in the child ScreenFlashOverlayImage");

        if (images.attackerLocationIndicatorImage == null)
            Debug.Log("This object is missing the image component in the child AttackerLocationIndicatorImage");

        if (playerHealthManager == null)
            Debug.Log("The object is missing player's health manager.");

        if(player == null)
            Debug.Log("Player not found");

        subscribeToEvents();

        setImageAlpha(images.damageOverlayImage, 0);
        setImageAlpha(images.screenFlashOverlayImage, 0);
        setImageAlpha(images.attackerLocationIndicatorImage, 0);
    }


    void Update() {

        updateScreenFlashOverlay();
    }

    void updateScreenFlashOverlay() {

        Vector4 newColor = images.screenFlashOverlayImage.color;

        if (isScreenFlashing()) {

            newColor.w = Mathf.Sin(screenFlashSpeed * (Time.time - timeScreenStartedFlashing));

            //normalize the color into the range [0, 1]
            newColor.w = (newColor.w + 1) / 2;

        } else {

            //screens not flashing so get rid of lash effect completely
            newColor.w = 0;
        }

        images.screenFlashOverlayImage.color = newColor;
    }

    void OnEnable() {

        subscribeToEvents();
    }

    void OnDisable() {

        unsubscribeFromEvents();
    }

    void subscribeToEvents() {

        playerHealthManager.onHit += onHit;
        playerController.onHit += onHit;

    }

    void unsubscribeFromEvents() {

        playerHealthManager.onHit -= onHit;
        playerController.onHit -= onHit;
    }

    //this is a response to the event from the health manager when the player receives damage
    void onHit(int damageReceived, float fractionHealthRemaining) {

        //player just got hit, start by setting the desired transparency of the overlay
        //when players healht is 0, effect should be fully visable, when player has 100% health, it should be invisible
        setImageAlpha(images.damageOverlayImage, Mathf.Clamp(1 - fractionHealthRemaining, 0, 1));

        //now we want to try applying a flashing effect, only if the screen isn't flashing already
        if (!isScreenFlashing())
            startScreenFlashEffect();
    }
    
    //this is a response to the event from the player when he gets hit by some entity (collision event)
    void onHit(GameObject collisionFunctionCallerObject, Collider otherObjectCollider) { 

        lastAttacker = otherObjectCollider.gameObject;

        drawAttackerLocationIndicator();
        StartCoroutine(updateAttackerLocationIndicator());
    }

    //rotate the attacker location indicator to point towards the position of the last attacker to hit the player
    IEnumerator updateAttackerLocationIndicator() {

        //how often the indicator's rotation should be updated
        float updateDelay = attackerLocationIndicatorDisplayLength / 20;

        //cache the transform of the last attacker
        Transform attackerLocationIndicatorTransform = images.attackerLocationIndicatorImage.transform;

        //set the transparency to 1 so we can see the location indicator
        setImageAlpha(images.attackerLocationIndicatorImage, 1);

        while (isAttackerLocationIndicatorDrawn()) {

            Vector3 attackerPosition = lastAttacker.transform.position;
            Vector3 playerPosition = player.transform.position;
            Vector3 playerDirection = player.transform.forward;

            //determine what direction the attack came from, we only care about its position on the x-z plane
            Vector2 directionToAttacker = new Vector2(attackerPosition.x, attackerPosition.z) - new Vector2(playerPosition.x, playerPosition.z);
            directionToAttacker.Normalize();

            //direction of player from birds eye view
            Vector2 playerDirectionBirdsEye = new Vector2(playerDirection.x, playerDirection.z);

            float dotAngle = Vector2.Dot(playerDirectionBirdsEye, directionToAttacker);
            float detAngle = playerDirectionBirdsEye.x * directionToAttacker.y - directionToAttacker.x * playerDirectionBirdsEye.y;
            float angle = Mathf.Atan2(detAngle, dotAngle) * 180.0f / 3.1415f;

            attackerLocationIndicatorTransform.rotation = Quaternion.Euler(0, 0, angle);

            yield return new WaitForSeconds(updateDelay);
        }

        //hide the indicator
        setImageAlpha(images.attackerLocationIndicatorImage, 0);
    }

    void setImageAlpha(Image image, float alpha) {

        Vector4 newColor = image.color;
        newColor.w = alpha;
        image.color = newColor;
    }

    void startScreenFlashEffect() {

        timeScreenStartedFlashing = Time.time;
    }

    bool isScreenFlashing() {

        return Time.time <= (timeScreenStartedFlashing + screenFlashLength);
    }

    void drawAttackerLocationIndicator() {

        timeAttackerLocationIndicatorStartedDrawing = Time.time;
    }

    bool isAttackerLocationIndicatorDrawn() {

        return Time.time <= (timeAttackerLocationIndicatorStartedDrawing + attackerLocationIndicatorDisplayLength);
    }
}
