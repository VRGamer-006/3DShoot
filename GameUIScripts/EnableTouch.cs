using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnableTouch : MonoBehaviour {
	public List<PlayerLook> lookScripts;
	
	void OnPress (bool pressed) {
		foreach(PlayerLook lookScript in lookScripts) {
			lookScript.enableTouch = !pressed;
		}
	}
}
