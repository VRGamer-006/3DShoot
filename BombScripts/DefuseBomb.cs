using UnityEngine;
using System.Collections;

public class DefuseBomb : MonoBehaviour {

	public float speed = 1.5f;
	private float tempTime = 0;
	private bool startDefuseBomb;
	public GameObject bombProgressBar;
	public UISprite foreground;
	public FireButton fireBtn;
	public WeaponManager weaponManager;
	public WeaponCrosshair crosshairScript;
	public GameObject gameWin;
	
	void OnTriggerStay(Collider other) {
        if (GameObject.FindWithTag("Enemy")){
            return;
        }
        if (other.gameObject.tag == "Player") {
			crosshairScript.canShowCrosshair = false;
			weaponManager.selectedWeapon.defusingBomb = true;
			
			bombProgressBar.SetActive(true);
			
			if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
				startDefuseBomb = fireBtn.defuseBomb;
			}
			else {
				startDefuseBomb = Input.GetMouseButton(0);
			}
			if(startDefuseBomb) {
				tempTime = Mathf.Lerp(tempTime, 1, speed * Time.deltaTime);
			}
			else {
				tempTime = Mathf.Lerp(tempTime, 0, speed * Time.deltaTime);
			}
			
			if(tempTime>0.99f) {
				tempTime = 1;
				gameWin.SendMessage("GameWin", SendMessageOptions.RequireReceiver);
			}
			foreground.fillAmount = tempTime;
		}         
    }
	
	  void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
			crosshairScript.canShowCrosshair = true;
			weaponManager.selectedWeapon.defusingBomb = false;
			tempTime = 0;
			bombProgressBar.SetActive(false);
		}
    }
}
