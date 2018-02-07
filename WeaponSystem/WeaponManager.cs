using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class WeaponManager : MonoBehaviour {
	public List<Weapon> allWeapons;
	private Weapon[] weaponEquipment = new Weapon[2];
	public float switchTime = 0.5f;
	
	[HideInInspector]
	public Weapon selectedWeapon;
	
	private int index = 0;
	public AudioClip weaponChangeAudio;
	
	private bool canSwitch;
	private bool isNextWeapon;
	private bool switchWeaponBtnClick;
	
	public ChangeSprite changeSprite;
	
	void Awake () {
		string weaponName = PlayerPrefs.GetString("weapen","Deagle");
		foreach(Weapon weapon in allWeapons) {
			weapon.gameObject.SetActiveRecursively(false);
			if(weapon.name == "Deagle") {
				weaponEquipment[0] = weapon;
			}
			
			if(weaponName != "Deagle" && weaponName == weapon.name) {
				weaponEquipment[1] = weapon;
			}
			else if(weaponName == "Deagle"){
				weaponEquipment[1] = null;
			}
		}
		
		selectedWeapon = weaponEquipment[0];
		TakeFirstWeapon(weaponEquipment[0].gameObject);
	}
	
	void Update () {
		if(Time.timeScale < 0.01f) {
			return;
		}
		
		selectedWeapon = weaponEquipment[index];
		if(weaponEquipment[1] == null) {
			changeSprite.canChangeSprite = false;
			return;
		}
		else {
			changeSprite.canChangeSprite = true;
		}
		
		isNextWeapon = (Input.GetKeyDown("2") || switchWeaponBtnClick) ? true : false;
		switchWeaponBtnClick = false;
		if(isNextWeapon && canSwitch) {
			StartCoroutine(SwitchWeapons(weaponEquipment[index].gameObject, weaponEquipment[(index+1)%2].gameObject));
			index = (index+1)%2;
		}
		
		if(Input.GetKeyDown("1") && canSwitch) {
			StartCoroutine(SwitchWeapons(weaponEquipment[index].gameObject, weaponEquipment[Mathf.Abs(index-1)%2].gameObject));
			index = Mathf.Abs(index-1)%2;
		}
	}
	
	void TakeFirstWeaponSoundPlay() {
        if(CameraAdaption.sound)
        {
            audio.clip = weaponChangeAudio;
            audio.Play();
        }
	}
	
	void TakeFirstWeapon(GameObject weapon) {
		weapon.SetActiveRecursively(true);
		weapon.SendMessage("SelectWeapon");
		canSwitch = true;
	}
	
	IEnumerator SwitchWeapons(GameObject currentWeapon, GameObject nextWeapon) {
		canSwitch = false;
		if(currentWeapon.active == true){
			currentWeapon.SendMessage("DeselectWeapon");
		}
		yield return new WaitForSeconds(switchTime);
		if(CameraAdaption.sound)
        {
            audio.clip = weaponChangeAudio;
            audio.Play();
        }		
		currentWeapon.SetActiveRecursively(false);
		nextWeapon.SetActiveRecursively(true);
		nextWeapon.SendMessage("SelectWeapon");
		canSwitch = true;
	}
	void OnFire(bool isFire) {
		if(selectedWeapon.singleFire && isFire){
			selectedWeapon.fireBtnClickDown = true;
		}
		else if(!selectedWeapon.singleFire){
			selectedWeapon.fireBtnClick = isFire;
		}
	}
	
	void OnAim(bool isAim) {
		selectedWeapon.aimBtnClick = isAim;
	}
	
	void OnReload(bool isReload) {
		selectedWeapon.reloadBtnClick = isReload;
	}
	
	void OnSwitchWeapon(bool isSwitchWeapon) {
		switchWeaponBtnClick = isSwitchWeapon;
	}
}
