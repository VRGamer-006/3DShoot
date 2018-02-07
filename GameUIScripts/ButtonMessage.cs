using UnityEngine;
using System.Collections;

public class ButtonMessage : MonoBehaviour {
	public GameObject messaageToObj;
	public string functionName;

    void OnPress (bool isPressed) {
		messaageToObj.SendMessage(functionName, isPressed, SendMessageOptions.RequireReceiver);
	}
}
