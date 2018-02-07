using UnityEngine;
using System.Collections;

public class TimeDisplay : MonoBehaviour {
	public UISprite firstNum;
	public UISprite secondNum;
	public UISprite threeNum;
	public UISprite fourNum;
	
	public int totalTime = 600;
	private int startTime;
	private int countTime;
	public static int showTime;
	private bool isStartTime = false;
	
	public GameObject gameLose;
	public GameObject bomb;
	
	void Awake () {
		totalTime = PlayerPrefs.GetInt("time", 600);
		showTime = totalTime;
	}
	
	void Start () {
		StartCoroutine(CheckTimeOut());
	}
	
	void Update () {
		if(isStartTime && showTime > 0) {
			countTime = (int)Time.time - startTime;
			showTime = totalTime - countTime;
		}
		ChangeSprite(showTime);
	}
	
	void ChangeSprite(int time) {
		int minute = time / 60;
		int seconds = time % 60;
		int num1 = minute / 10;
		int num2 = minute % 10;
		int num3 = seconds / 10;
		int num4 = seconds % 10;
		firstNum.spriteName = "d" + num1;
		secondNum.spriteName = "d" + num2;
		threeNum.spriteName = "d" + num3;
		fourNum.spriteName = "d" + num4;
	}
	
	void OnStart() {
		isStartTime = !isStartTime;
		startTime = (int)Time.time;
		totalTime = showTime;
	}
	
	IEnumerator CheckTimeOut() {
		
		while(showTime >= 0) {
			yield return 0;
			if(showTime == 0) {
				bomb.SendMessage("OnExplode", SendMessageOptions.RequireReceiver);
				yield return new WaitForSeconds(4f);
				gameLose.SendMessage("GameLose", SendMessageOptions.RequireReceiver);
			}
		}
	}
		
	public float GetTime() {
		return showTime;
	}
}