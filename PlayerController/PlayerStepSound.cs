using UnityEngine;
using System.Collections;

public class PlayerStepSound : MonoBehaviour {
	public AudioClip[] walkSounds;
	public float walkStepLength = 0.4f;
	public float runStepLength = 0.32f;
	public float crouchStepLength = 0.5f;
	
	private CharacterController controller;
	private PlayerController motor;
	private float lastStep = -10.0f;
	private float stepLength;
	
	void Awake () {
		stepLength = walkStepLength;
		controller = GetComponent<CharacterController>();
		motor = GetComponent<PlayerController>();
	}
	
	void FixedUpdate () {
		if(motor.walking && motor.grounded && !motor.crouch) {
			PlayStepSound();
			stepLength = walkStepLength;
		}
		if(motor.running && motor.grounded) {
			PlayStepSound();
			stepLength = runStepLength;
		}
		if(motor.walking && motor.crouch && motor.grounded) {
			PlayStepSound();
			stepLength = crouchStepLength;
		}
	}
	
	void PlayStepSound() {
		if(Time.time > stepLength + lastStep) {
            if(CameraAdaption.sound) {
                audio.clip = walkSounds[Random.Range(0, walkSounds.Length)];
                audio.Play();
            }			
			lastStep = Time.time;
		}
	}
}
