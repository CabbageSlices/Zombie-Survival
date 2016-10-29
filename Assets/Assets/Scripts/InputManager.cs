using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public delegate void InputResponse();
    public delegate void ButtonDownResponse(bool isButtonHeldDown);

    public event ButtonDownResponse isPressingAim;
    public event InputResponse onWeaponPrimary;
    public event InputResponse onReload;
    public event InputResponse onAimDownSight;
	
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
	}
}
