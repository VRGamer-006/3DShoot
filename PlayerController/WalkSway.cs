using UnityEngine;
using System.Collections;

public class WalkSway : MonoBehaviour {
	
	public float walkBobbingSpeed = 0.21f;
	public float runBobbingSpeed = 0.35f;
	public float idleBobbingSpeed = 0.1f;
	public float bobbingAmount = 0.1f;
	public float smooth= 1;
	private Vector3 midpoint;
	private GameObject player;
	private float timer = 0.0f;
	private float bobbingSpeed; 
	private PlayerController motor;
	private float BobbingAmount;
	
	void Awake (){
		player = GameObject.FindWithTag("Player");
		motor = player.GetComponent<PlayerController>();
		midpoint = transform.localPosition;
	
	}
	 
	 void FixedUpdate () { 
	    float waveslice = 0.0f;
	    float waveslice2 = 0.0f;
	    Vector3 currentPosition = Vector3.zero;
	    
	    float tempWalkSpeed = 0;
	    float tempRunSpeed = 0;
	    float tempIdleSpeed = 0;
	    
	    if(Time.timeScale == 1){
	    	if(tempWalkSpeed != walkBobbingSpeed || tempRunSpeed != runBobbingSpeed || tempIdleSpeed != idleBobbingSpeed){
				tempWalkSpeed = walkBobbingSpeed;
				tempRunSpeed = runBobbingSpeed;
				tempIdleSpeed = idleBobbingSpeed;
			}
		}else{
			tempWalkSpeed = walkBobbingSpeed*(Time.fixedDeltaTime/0.02f);
			tempRunSpeed = runBobbingSpeed*(Time.fixedDeltaTime/0.02f);
			tempIdleSpeed = idleBobbingSpeed*(Time.fixedDeltaTime/0.02f);
		}
	
		waveslice = Mathf.Sin(timer*2);
		waveslice2 = Mathf.Sin(timer);
		timer = timer + bobbingSpeed;
		if (timer > Mathf.PI * 2) {
		  timer = timer - (Mathf.PI * 2);
		}
	    if (waveslice != 0) {
			float tempTranslateChange = waveslice * BobbingAmount;
			float tempTranslateChange2 = waveslice2 * BobbingAmount;
			float totalAxes = Mathf.Clamp (1.0f, 0.0f, 1.0f);
			float translateChange = totalAxes * tempTranslateChange;
			float translateChange2 = totalAxes * tempTranslateChange2;
			
			if(motor.grounded){
				currentPosition.y = midpoint.y + translateChange;
				currentPosition.x = midpoint.x + translateChange2;
	   		}
	   		
	    }else{
	    	currentPosition = midpoint;
	    } 

		if (motor.walking && !motor.running) {
			bobbingSpeed = tempWalkSpeed;
			BobbingAmount = bobbingAmount;
		}
		if(motor.running) {
			bobbingSpeed = tempRunSpeed;
			BobbingAmount = bobbingAmount;
		}
		
		if(!motor.running && !motor.walking){
			bobbingSpeed = tempIdleSpeed;
			BobbingAmount = bobbingAmount*0.3f;
			
		}
		
		float i = 0;
		i += Time.deltaTime * smooth;
	
		transform.localPosition = Vector3.Lerp(transform.localPosition, currentPosition, i);
	 }
}
