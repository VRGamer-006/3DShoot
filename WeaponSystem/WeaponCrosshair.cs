using UnityEngine;
using System.Collections;

public class WeaponCrosshair : MonoBehaviour {

	public Texture2D crosshairTexture;
	public float length = 15;
	public float width = 1;
	public bool dynamicCrosshair = true;
	public float crosshairResponce = 50;
	public float defaultDistance = 20;
	public float smooth = 0.3f;
	private bool crosshair = true;
	private Texture textu;
	private GUIStyle lineStyle;
	private float distance;
	private float currentDistance;
	private PlayerController motor;
	private WeaponManager weaponManager;
	private Weapon weapon;
	
	public bool canShowCrosshair;
	
	void Awake () {
		lineStyle = new GUIStyle();
		lineStyle.normal.background = crosshairTexture;
		motor = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
		weaponManager = GameObject.FindWithTag("WeaponManager").GetComponent<WeaponManager>();
	}
	
	void Update(){
		if(weaponManager){
			if(weaponManager.selectedWeapon)
				weapon = weaponManager.selectedWeapon.GetComponent<Weapon>();
		}
		
		if(Time.timeScale < 0.01f)
			return;
			
		if(dynamicCrosshair){
			bool fireInput = Input.GetMouseButtonDown(0);

			if(weapon && (fireInput || weapon.fire)){
				if(weapon.singleFire){
					if(fireInput && weapon.canFire && !weapon.isReload && !weapon.noBullets){
						if(distance < crosshairResponce*4){
							distance = distance + crosshairResponce;
						}
					}else{
						distance = Mathf.Lerp(distance, defaultDistance, Time.deltaTime/smooth);
					}
				}else{
					if(weapon.fire && !weapon.noBullets){
						currentDistance = crosshairResponce*2;
					}else{
						currentDistance = defaultDistance;	
					}
					distance = Mathf.Lerp(distance, currentDistance, Time.deltaTime/smooth);
				}
			}else{
				currentDistance = defaultDistance;
				distance = Mathf.Lerp(distance, currentDistance, Time.deltaTime/smooth);
			}
		}else{
			distance = defaultDistance;
		}
		
		if(weapon)
			if(weapon.aimed){
				crosshair = false;
			}else{
				crosshair = true;
			}
	}
	
	void OnGUI () {
		if(!(distance > (Screen.height/2)) && canShowCrosshair && crosshair){
		
			GUI.Box(new Rect((Screen.width - distance)/2 - length, (Screen.height - width)/2, length, width), textu, lineStyle);
			GUI.Box(new Rect((Screen.width + distance)/2, (Screen.height- width)/2, length, width), textu, lineStyle);
		
			GUI.Box(new Rect((Screen.width - width)/2, (Screen.height - distance)/2 - length, width, length), textu, lineStyle);
			GUI.Box(new Rect((Screen.width - width)/2, (Screen.height + distance)/2, width, length), textu, lineStyle);
		}
	}
}
