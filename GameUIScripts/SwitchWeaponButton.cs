using UnityEngine;
using System.Collections;

public class SwitchWeaponButton : MonoBehaviour {
	public GameObject messaageToObj;
	public string functionName;
	
	 void OnClick (bool isPressed) {
		messaageToObj.SendMessage(functionName, isPressed, SendMessageOptions.RequireReceiver);
	}
}
