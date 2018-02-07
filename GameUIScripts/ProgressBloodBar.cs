using UnityEngine;
using System.Collections;

public class ProgressBloodBar : MonoBehaviour {
  	public UISprite foregroundSprite;
	public UISprite backgroundSprite;
    public UILabel displayTxt;
	
	public GameObject timeOutMessageTo;
	
	private Color c = Color.white;
	
	private float lastBlood;
	private float currentBlood;
    private float maxBlood = 100;

	void Awake() {
		currentBlood = maxBlood;
		lastBlood = currentBlood;
    }
	
	void FixedUpdate() {
		currentBlood = lastBlood;
		UpdateDisplay();
		CheckBlood();
    } 
   
    void UpdateDisplay() {  
		float tempBlood = Mathf.Clamp01(currentBlood / maxBlood);
		foregroundSprite.fillAmount = tempBlood;
		if(tempBlood < 0.6f) {
			c.b = Mathf.Lerp(c.b, 0, (0.6f - tempBlood)/0.2f);
		}
		if(tempBlood < 0.4f) {
			c.g = Mathf.Lerp(c.g, 0, (0.4f - tempBlood)/0.4f);
			
			if(backgroundSprite.color == Color.white) {
				backgroundSprite.color = Color.red;
			}
			else {
				if(currentBlood != 0) {
					backgroundSprite.color = Color.white;
				}
			}
		}
		foregroundSprite.color = c;
		displayTxt.text = string.Format("{0}/{1}", currentBlood, maxBlood);
    }
	
	public void CalLastBlood(float loseBlood) {
		if(loseBlood < currentBlood) {
			lastBlood = currentBlood - loseBlood;
		}
		else {
			lastBlood = 0;
		}
	}
	
	void CheckBlood() {
		if(currentBlood == 0) {
			timeOutMessageTo.SendMessage("GameLose", SendMessageOptions.RequireReceiver);
		}
	}
}