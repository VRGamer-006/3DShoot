using UnityEngine;
using System.Collections;

public class FireButton : MonoBehaviour {

	public GameObject messaageToObj;
	public string functionName;
	
	[HideInInspector]
	public bool defuseBomb;

    void OnPress (bool isPressed) {
		messaageToObj.SendMessage(functionName, isPressed, SendMessageOptions.RequireReceiver);
		defuseBomb = isPressed;
	}
}
