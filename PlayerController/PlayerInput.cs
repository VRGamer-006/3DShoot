using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour {
	private PlayerController motor;
	public NGUIJoystick joystick;
	private Vector3 directionVector = Vector3.zero;
	
	private bool jumpBtnClick = false;
	private bool crouchBtnClick = false;
	
	void Awake () {
		motor = GetComponent<PlayerController>();
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("MenuScene");
        }
		directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) {
			directionVector = new Vector3(joystick.position.x, 0, joystick.position.y);
		}
		
		if(directionVector != Vector3.zero) {
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			directionLength = Mathf.Min(1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		
		motor.inputMoveDirection = transform.rotation * directionVector;
		
		if(!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
			motor.inputJump = Input.GetKeyDown(KeyCode.Space);
			
			motor.inputRun = Input.GetKey(KeyCode.LeftShift);
			
			motor.inputCrouch = Input.GetKeyDown(KeyCode.C);
		}
		else {
			motor.inputJump = jumpBtnClick;
			jumpBtnClick = false;
			
			motor.inputCrouch = crouchBtnClick;
			crouchBtnClick = false;
		}
	}
	
	void OnJumping() {
		jumpBtnClick = true;
	}
	
	void OnCrouch() {
		crouchBtnClick = true;
	}
}
