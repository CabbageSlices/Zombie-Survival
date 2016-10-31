using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public delegate void InputResponse();
    public delegate void ButtonDownResponse(bool isButtonHeldDown);

    public event ButtonDownResponse isPressingAim;
    public event ButtonDownResponse isSprinting;
    public event InputResponse onWeaponPrimary;
    public event InputResponse onReload;
    public event InputResponse onAimDownSight;
	
    private 

	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.R) && onReload != null)
            onReload();

        if(Input.GetMouseButtonDown(1) && onAimDownSight != null)
            onAimDownSight();

        if (Input.GetMouseButton(0) && onWeaponPrimary != null)
            onWeaponPrimary();
        

        if (isPressingAim != null)
            isPressingAim(Input.GetMouseButton(1));

        //player is sprinting if he is holding the sprint key and moving 
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if(isSprinting != null)
            isSprinting(isMoving && Input.GetKey(KeyCode.LeftShift));
	}
}
