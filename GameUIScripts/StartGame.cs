using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {
	public GameObject timer;
	public string functionName;
	
	void OnClick() {
		timer.SendMessage(functionName, SendMessageOptions.RequireReceiver);
	}
}
