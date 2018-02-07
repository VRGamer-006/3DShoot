using UnityEngine;
using System.Collections;
public class PlayerLook : MonoBehaviour {
	public enum RotationAxes { MouseX, MouseY };
	public RotationAxes axes = RotationAxes.MouseX;
	public float sensitivity = 4;
	public float aimSensitivity = 2;
	
	[HideInInspector]
	public float sensitivityX = 15.0f;
	[HideInInspector]
	public float sensitivityY = 15.0f;
	public float minY = -80;
	public float maxY = 80;
	
	[HideInInspector]
	public bool enableTouch = true;
	public int touchIndex = -1;
	private float inputX;
	private float inputY;
	float rotationY = 0;
	
	private WeaponManager weaponManager;
	private Weapon weaponScript;
	
	[HideInInspector]
	public float currentSensitivity;
	
	void Awake (){
		weaponManager = GameObject.FindWithTag("WeaponManager").GetComponent<WeaponManager>();
	}
	
	void Update () {
		if(!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
			inputX = Input.GetAxis("Mouse X");
			inputY = Input.GetAxis("Mouse Y");
		}
		else {
			if(enableTouch) {

				for(int i=0;i<Input.touchCount;i++) {
					Touch touch = Input.GetTouch(i);
					if(touch.fingerId == touchIndex) continue;
					else {
						inputX = Mathf.Clamp(touch.deltaPosition.x, -1, 1);
						inputY = Mathf.Clamp(touch.deltaPosition.y, -1, 1);
					}
				}
			}
		}
		
		if(axes == RotationAxes.MouseX) {
			transform.Rotate(0, inputX * sensitivityY, 0);
		}
		else if(axes == RotationAxes.MouseY) {
			rotationY += inputY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minY, maxY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		} 
		
		if(weaponManager.selectedWeapon){
			weaponScript = weaponManager.selectedWeapon.GetComponent<Weapon>();
		}

		if(weaponScript && weaponScript.aimed){
			currentSensitivity = aimSensitivity;
		}else{
			currentSensitivity = sensitivity;
		}
		sensitivityX = currentSensitivity;
		sensitivityY = currentSensitivity;
	}
	
	public void Recoil(float amount){
		rotationY += amount;
	}
}
