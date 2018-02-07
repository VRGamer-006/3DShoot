using UnityEngine;
using System.Collections;

public class BackToMenu : MonoBehaviour {
	
	void OnClick() {
		Time.timeScale = 1;
		Application.LoadLevel("MenuScene");
	}
}
