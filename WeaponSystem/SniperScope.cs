using UnityEngine;
using System.Collections;

public class SniperScope : MonoBehaviour {
	public GameObject NGUIScope;
	public GameObject[] objectsToDeactive;
	private Weapon weapon;

	void Awake () {
		weapon = gameObject.GetComponent<Weapon>();
	}

	void OnGUI () {
		if(weapon.aimed) {
			NGUIScope.SetActive(true);
			for(int i=0; i<objectsToDeactive.Length;i++) {
				objectsToDeactive[i].SetActiveRecursively(false);
			}
		}
		else {
			NGUIScope.SetActive(false);
			for(int j=0;j<objectsToDeactive.Length;j++) {
				objectsToDeactive[j].SetActiveRecursively(true);
			}
		}
	}
}
