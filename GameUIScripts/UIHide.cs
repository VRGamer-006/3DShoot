using UnityEngine;
using System.Collections;

public class UIHide : MonoBehaviour {
	public GameObject[] hideObjs;
    public UILabel moneyLabel;
	private GameObject player;
	private PlayerLook playerLook;
	private PlayerLook headLook;
	private PlayerInput inputScript;
	private WeaponCrosshair crosshairScript;
	
	public GameObject weaponManager;
	
	public GameObject gameMessagePanel;
	public GameObject gameWinPanel;
	public GameObject gameLosePanel;
	public ProgressBloodBar bloodScript;

    bool result;
	void Awake () {
		 HideObjs();
         result = false;
	}
	
	void HideObjs() {
		Time.timeScale = 0;
		foreach(GameObject go in hideObjs) {
				go.SetActive(false);
		}
		player = GameObject.Find("Player");
		playerLook = player.GetComponent<PlayerLook>();
		inputScript = player.GetComponent<PlayerInput>();
		headLook = GameObject.Find("HeadLook").GetComponent<PlayerLook>();
		crosshairScript = player.GetComponentInChildren<WeaponCrosshair>();
		playerLook.enabled = false;
		inputScript.enabled = false;
		headLook.enabled = false;
		crosshairScript.enabled = false;
	}
	
	void OnStartGame() {
		Time.timeScale = 1;
		
		playerLook.enabled = true;
		inputScript.enabled = true;
		headLook.enabled = true;
		crosshairScript.enabled = true;
		
		weaponManager.SendMessage("TakeFirstWeaponSoundPlay", SendMessageOptions.RequireReceiver);
		
		foreach(GameObject go in hideObjs) {
			if(!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				 && (go.name == "Buttons" || go.name == "Joystick")){
				continue;
			}
			
			go.SetActive(true);
			
			if(go.name == "Timer") {
				go.SendMessage("OnStart", SendMessageOptions.RequireReceiver);
			}
		}
		gameMessagePanel.SetActive(false);
	}
	
	void GameLose () {
        if(result) {
            return;
        }
		HideObjs();
		gameLosePanel.SetActive(true);
        result = true;
	}
	
	void GameWin () {       
        if(result) {
            return;
        }
        float money =  PlayerPrefs.GetFloat("money", 300f);
        money += TimeDisplay.showTime;
        PlayerPrefs.SetFloat("money", money);   
		HideObjs();
		gameWinPanel.SetActive(true);
        moneyLabel.text = "$  " + TimeDisplay.showTime.ToString();
        result = true;
	}
}
