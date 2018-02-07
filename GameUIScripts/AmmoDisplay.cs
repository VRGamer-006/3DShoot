using UnityEngine;
using System.Collections;

public class AmmoDisplay : MonoBehaviour {
	private WeaponManager weaponManager;
	private UILabel label;
	void Start () {
		weaponManager = GameObject.FindWithTag("WeaponManager").GetComponent<WeaponManager>();
		label = GetComponent<UILabel>();
	}
	
	void Update () {
		if(!weaponManager) return;
		if(weaponManager.selectedWeapon.gunType == Weapon.GunType.MACHINE_GUN) {
			label.text = weaponManager.selectedWeapon.machineGun.bulletsLeft + " | " +
							weaponManager.selectedWeapon.machineGun.clips;
		}
		else {
			label.text = weaponManager.selectedWeapon.shotGun.bulletsLeft + " | " +
							weaponManager.selectedWeapon.shotGun.clips;
		}
	}
}
