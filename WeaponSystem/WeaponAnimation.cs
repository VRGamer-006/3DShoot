using UnityEngine;
using System.Collections;

public class WeaponAnimation : MonoBehaviour {
	public float fireAnimationSpeed = 1;
	public float takeInOutSpeed = 1;
	public float reloadMiddleRepeat = 3;
	public bool isSniperRifle = false;

	void Awake () {
		animation.Play("Idle");
		animation.wrapMode = WrapMode.Once;
	}

	void Fire(){
		animation.Rewind("Fire");
		animation["Fire"].speed = fireAnimationSpeed;
		animation.Play("Fire");
	}

	void Reloading(float reloadTime) {
		if(!isSniperRifle) {
			animation.Stop("Reload");
			animation["Reload"].speed = animation["Reload"].clip.length/reloadTime;
			animation.Rewind("Reload");
			animation.Play("Reload");
		}
		else {
			AnimationState newReload1 = animation.CrossFadeQueued("Reload_1_3");
			newReload1.speed = animation["Reload_1_3"].clip.length/reloadTime;
			for(int i = 0; i < reloadMiddleRepeat; i++){
		 		AnimationState newReload2 = animation.CrossFadeQueued("Reload_2_3");
				newReload2.speed = animation["Reload_2_3"].clip.length/reloadTime;
			}
			AnimationState newReload3 = animation.CrossFadeQueued("Reload_3_3");
			newReload3.speed = animation["Reload_3_3"].clip.length/reloadTime;
		}
	}

	void TakeIn(){
		animation.Rewind("TakeIn");
		animation["TakeIn"].speed = takeInOutSpeed;
		animation["TakeIn"].time = 0;
		animation.Play("TakeIn");
	}

	void TakeOut(){
		animation.Rewind("TakeOut");
		animation["TakeOut"].speed = takeInOutSpeed;
		animation["TakeOut"].time = 0;
		animation.Play("TakeOut");
	}
}
