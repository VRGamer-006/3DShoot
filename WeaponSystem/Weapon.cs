using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {
	[HideInInspector]
	public bool aimed;
	[HideInInspector]
	public bool fire;
	[HideInInspector]
	public bool canAim;
	[HideInInspector]
	public bool isReload;
	[HideInInspector]
	public bool noBullets;
	[HideInInspector]
	public bool canFire;
	
	[HideInInspector]
	public bool aimBtnClick;
	[HideInInspector]
	public bool fireBtnClick;
	[HideInInspector]
	public bool fireBtnClickDown;
	[HideInInspector]
	public bool reloadBtnClick;
	[HideInInspector]
	public bool defusingBomb;
		
	public bool singleFire;
	public bool recoil;
	
	private PlayerController motor;
	private GameObject player;
	private CharacterController controller;
	private PlayerLook mouseLook;
	
	private WalkSway walkSway;
	private float defaultBobbingAmount;
	private GameObject managerObject;
	
	public enum GunType {MACHINE_GUN, SHOTGUN}
	public GunType gunType;
	
	[System.Serializable]
	public class AimVariables {
		public Vector3 aimPosition = Vector3.zero;
		public float smoothTime = 0.05f;
		public float toFov = 45;
		public float aimBobbingAmount;
		public bool playAnimation;
	}
	public AimVariables aim = new AimVariables();
	private float defaultFov;
	private Vector3 defaultPosition;
	private float currentFov;
	private Vector3 currentPosition;
	
	public Transform firePoint;
	
	[System.Serializable]
	public class MachineGun{
		public Transform bullet;
		public GameObject muzzleFlash;
		public AudioClip fireSound;
		public AudioClip reloadSound;
		public Light pointLight;
		public float fireRate = 0.05f;
		public int bulletsPerClip = 40;
		public int clips = 15;
		public int bulletsLeft;
		public float reloadTime = 1.0f;
		public float noAimErrorAngle = 3.0f;
		public float aimErrorAngle = 0.0f;
	}
	public MachineGun machineGun = new MachineGun();
	[HideInInspector]
	public float errorAngle;
	private float nextFireTime = 0.0f;
	
	[System.Serializable]
	public class ShotGun {
		public Transform bullet;
		public AudioClip fireSound;
		public AudioClip reloadSound;
		public ParticleEmitter smoke;
		public int fractions = 5;
		public float errorAngle = 3;
		public float fireRate = 1;
		public float reloadTime = 2;
		public int bulletsPerClip = 40;
		public int bulletsLeft;
		public int clips = 15;
	}
	public ShotGun shotGun = new ShotGun();
	
	[System.Serializable]
	public class RotationReal{
		public float RotationAmplitude = 2;
		public float smooth = 7;
	}
	public RotationReal rotationRealism = new RotationReal();
	private float currentAnglex;
	private float currentAngley;
	
	[System.Serializable]
	public class SmoothMov {
		public  float maxAmount = 0.5f;
		public  float Smooth = 3.0f;
	}
	public SmoothMov SmoothMovement = new SmoothMov();
	private Vector3 defaultPos;
	
	[System.Serializable]
	public class CameraRecoil{
		public float recoilPower = 0.5f;
		public float shakeAmount = 6;
		public float smooth = 3;
	}
	public CameraRecoil cameraRecoil = new CameraRecoil();
	
	private float lastShot = -10.0f;
	private Quaternion camDefaultRotation;
	private Quaternion camPos;
	
	void Awake(){
		player = GameObject.FindWithTag("Player");
		motor = player.GetComponent<PlayerController>();
		controller = player.GetComponent<CharacterController>();
		mouseLook = GameObject.FindWithTag("LookObject").GetComponent<PlayerLook>();
	}

	void Start () {
		managerObject = GameObject.FindWithTag("WeaponManager");
		walkSway = managerObject.GetComponent<WalkSway>();
		defaultBobbingAmount = walkSway.bobbingAmount;
		
		camDefaultRotation = Camera.main.transform.localRotation;
		
		defaultFov = Camera.main.fieldOfView;
		defaultPosition = transform.localPosition;
		
		if(gunType == GunType.MACHINE_GUN){
			MachineGunAwake();
		}
		
		if(gunType == GunType.SHOTGUN){
			ShotGunAwake();
		}
	}
	
	void Update () {
		if(Time.timeScale < 0.01f) {
			return;
		}
		Aiming();
		if(!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
			RotationRealism();
			SmoothMove();
		}

		if(recoil){
			CameraRecoilDo();
		}
		
		if(gunType == GunType.MACHINE_GUN){
			MachineGunFixedUpdate();
		}
	
		if(gunType == GunType.SHOTGUN){
			ShotGunFixedUpdate ();
		}
		
		if(motor.running){
			aimed = false;
		}
	}
	
	void LateUpdate(){
		if(Time.timeScale < 0.01f || defusingBomb) {
			return;
		}
		if(!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
			aimBtnClick = Input.GetButtonDown("Fire2");
			fireBtnClick = Input.GetButton("Fire1");
			fireBtnClickDown = Input.GetButtonDown("Fire1");
			reloadBtnClick = Input.GetKeyDown("r");
		}
		
		if(aimBtnClick && canAim && !motor.running){
			aimed = !aimed;
			if(aimBtnClick) {
				aimBtnClick = false;
			}
		}
		
		if(fireBtnClick && canFire && !singleFire){
			fire = true;
		}
		else{
			fire = false;
		}

		if(gunType == GunType.MACHINE_GUN){
			if(fireBtnClickDown && canFire && !isReload && singleFire){
				MachineGunFire();
				if(fireBtnClickDown) {
					fireBtnClickDown = false;
				}
			}else{
				MachineGunStopFire ();
			}
		}
			
		if(gunType == GunType.SHOTGUN){
			if(fireBtnClickDown && canFire && !isReload && singleFire){
				ShotGunFire ();
				if(fireBtnClickDown) {
					fireBtnClickDown = false;
				}
			}	
		}
		
		if(reloadBtnClick && !isReload && machineGun.clips > 0){
			if(reloadBtnClick) {
				reloadBtnClick = false;
			}
			if(gunType == GunType.MACHINE_GUN && machineGun.bulletsLeft != machineGun.bulletsPerClip){
				StartCoroutine(MachineGunReload());
			}
			if(gunType == GunType.SHOTGUN && shotGun.bulletsLeft != shotGun.bulletsPerClip){
				StartCoroutine(ShotGunReload());
			}
		}

	}
	
	void FirePointSetup(){
		Vector3 tempPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane));
  	 	firePoint.position = tempPos;
	}
	
	void MachineGunAwake(){
		machineGun.bulletsLeft = machineGun.bulletsPerClip;
		if(machineGun.muzzleFlash){
			machineGun.muzzleFlash.active = false;
		}
		canAim = true;
		canFire = true;
	}
	
	void MachineGunFixedUpdate (){
		if(fire && !isReload){
			MachineGunFire();
		}else{
			MachineGunStopFire();
			
			if(machineGun.muzzleFlash){
				machineGun.muzzleFlash.active = false;
			}
		}
	
		if(isReload){
			canAim = false;
		}
	}
	
	void MachineGunFire (){
		if (machineGun.bulletsLeft == 0)
			return;
		
		if (Time.time - machineGun.fireRate > nextFireTime){
			nextFireTime = Time.time - Time.deltaTime;
		}
        
		while( nextFireTime < Time.time && machineGun.bulletsLeft != 0){
			StartCoroutine(MachineGunOneShot());
			nextFireTime += machineGun.fireRate;
		}
	}
	
	void MachineGunStopFire (){
		motor.canRun = true;
	}
	
	IEnumerator MachineGunOneShot () {
		if(!aimed){
			FirePointSetup();
		}
		var oldRotation = firePoint.rotation;
		firePoint.rotation = Quaternion.Euler(Random.insideUnitSphere * errorAngle) * transform.rotation;
		if(!aimed){
			Instantiate (machineGun.bullet, firePoint.position, firePoint.rotation);
		}else{
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane));
			Instantiate (machineGun.bullet, pos, firePoint.rotation);
		}
		firePoint.rotation = oldRotation;
		lastShot = Time.time;
		machineGun.bulletsLeft--;
        if(CameraAdaption.sound)
        {
            audio.clip = machineGun.fireSound;
            audio.Play();
        }	
		StartCoroutine(MachineGunMuzzleFlash());
		if(aimed && aim.playAnimation){
			BroadcastMessage ("Fire", SendMessageOptions.DontRequireReceiver);
		}
		if(!aimed){
			BroadcastMessage ("Fire", SendMessageOptions.DontRequireReceiver);
		}
		
		if(recoil){
			mouseLook.Recoil(cameraRecoil.recoilPower);
			StartCoroutine(MachineGunCameraRecoil());
		}
	
		if(machineGun.clips > 0) {
			if (machineGun.bulletsLeft == 0){
				noBullets = true;
				yield return new WaitForSeconds(1);
				if(!isReload){
					StartCoroutine(MachineGunReload());
				}
			}
		}
	}
	
	IEnumerator MachineGunMuzzleFlash(){
		
		if(machineGun.muzzleFlash){
			machineGun.muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.left);
			machineGun.muzzleFlash.active = true;
		}
		if(machineGun.pointLight){
			machineGun.pointLight.enabled = true;
		}
		yield return new WaitForSeconds(0.04f);
		if(machineGun.muzzleFlash){
			machineGun.muzzleFlash.active = false;
		}
		if(machineGun.pointLight){
			machineGun.pointLight.enabled = false;
		}
	}
	
	IEnumerator MachineGunReload () {
		isReload = true;
		aimed = false;
		canAim = false;
		BroadcastMessage ("Reloading", machineGun.reloadTime, SendMessageOptions.DontRequireReceiver);
		
        if(CameraAdaption.sound)
        {
            audio.clip = machineGun.reloadSound;
            audio.Play();
        }	
		yield return new WaitForSeconds(machineGun.reloadTime);
	
		if (machineGun.clips > 0) {
			int difference = machineGun.bulletsPerClip-machineGun.bulletsLeft;
			if(machineGun.clips > difference ){
				machineGun.clips = machineGun.clips - difference;
				machineGun.bulletsLeft = machineGun.bulletsLeft + difference;
			}else{
				machineGun.bulletsLeft = machineGun.bulletsLeft + machineGun.clips;
				machineGun.clips = 0;
			}
			noBullets = false;
			isReload = false;
			canAim = true;
			motor.canRun = true;
		}
	}
	
	IEnumerator MachineGunCameraRecoil(){
		camPos = Quaternion.Euler (Random.Range(0, -cameraRecoil.shakeAmount), Random.Range(-cameraRecoil.shakeAmount, cameraRecoil.shakeAmount), 0);
		yield return new WaitForSeconds(0.05f);
		camPos = camDefaultRotation;
	}
	
	void ShotGunAwake(){
		shotGun.bulletsLeft = shotGun.bulletsPerClip;
		if(shotGun.smoke){
			shotGun.smoke.emit = false;
		}
		canAim = true;
		canFire = true;
	}
	
	void ShotGunFixedUpdate (){
		if(fire && !isReload){
			ShotGunFire();
		}else{
			ShotGunStopFire();
		}	
		
		if(isReload){
			canAim = false;
		}
	}
	
	void ShotGunFire (){
		if (shotGun.bulletsLeft == 0)
			return;
	
		if (Time.time - shotGun.fireRate > nextFireTime)
			nextFireTime = Time.time - Time.deltaTime;
		
		while( nextFireTime < Time.time && shotGun.bulletsLeft != 0){
			StartCoroutine(ShotGunOneShot());
			nextFireTime += shotGun.fireRate;
		}
	}
	
	void ShotGunStopFire (){
		motor.canRun = true;
	}
	
	IEnumerator ShotGunOneShot () {
		FirePointSetup();
		var oldRotation = firePoint.rotation;
		for (int i = 0;i < shotGun.fractions; i++) {
			firePoint.rotation = Quaternion.Euler(Random.insideUnitSphere * shotGun.errorAngle) * transform.rotation;
			var instantiatedProjectile = Instantiate (shotGun.bullet, firePoint.position, firePoint.rotation);
		}
		firePoint.rotation = oldRotation;
		lastShot = Time.time;
        if(CameraAdaption.sound) {
            audio.clip = shotGun.fireSound;
            audio.Play();
        }		
		shotGun.bulletsLeft--;
		
		if(aimed && aim.playAnimation){
			BroadcastMessage ("Fire", SendMessageOptions.DontRequireReceiver);
		}
		if(!aimed){
			BroadcastMessage ("Fire", SendMessageOptions.DontRequireReceiver);
		}
		StartCoroutine(ShotGunSmokeEffect());
		if(recoil){
			StartCoroutine(ShotGunCameraRecoil());
			mouseLook.Recoil(cameraRecoil.recoilPower);
		}
		
		if(shotGun.clips > 0)	
			if (shotGun.bulletsLeft == 0){
				noBullets = true;
				yield return new WaitForSeconds(1);
				if(!isReload){
					StartCoroutine(ShotGunReload());
				}
			}
	}
	
	IEnumerator ShotGunReload () {
		isReload = true;
		aimed = false;
		BroadcastMessage ("Reloading", shotGun.reloadTime, SendMessageOptions.DontRequireReceiver);

        if(CameraAdaption.sound)
        {
            audio.clip = shotGun.reloadSound;
            audio.Play();
        }		
		
		yield return new WaitForSeconds(shotGun.reloadTime);
	
		if (shotGun.clips > 0){
		var difference = shotGun.bulletsPerClip-shotGun.bulletsLeft;
			if(shotGun.clips > difference ){
				shotGun.clips = shotGun.clips - difference;
				shotGun.bulletsLeft = shotGun.bulletsLeft + difference;
			}else{
				shotGun.bulletsLeft = shotGun.bulletsLeft + shotGun.clips;
				shotGun.clips = 0;
			}
			noBullets = false;
			isReload = false;
			canAim = true;
			motor.canRun = true;
		}
	}
	
	IEnumerator ShotGunSmokeEffect(){
		if(!shotGun.smoke)
			yield break ;
		shotGun.smoke.emit = true;
		yield return new WaitForSeconds(0.3f);
		shotGun.smoke.emit = false;	
	}
	
	IEnumerator ShotGunCameraRecoil(){
		camPos = Quaternion.Euler (Random.Range(-cameraRecoil.shakeAmount*1.5f, -cameraRecoil.shakeAmount), Random.Range(cameraRecoil.shakeAmount/3, cameraRecoil.shakeAmount/2), 0);
		yield return new WaitForSeconds(0.1f);
		camPos = camDefaultRotation;
	}
	void Aiming(){
		if(aimed && !motor.running){
		 	currentPosition = aim.aimPosition;
			currentFov = aim.toFov;
			errorAngle = machineGun.aimErrorAngle;
			walkSway.bobbingAmount = aim.aimBobbingAmount;
		} 
		else {
			currentPosition = defaultPosition;
			currentFov = defaultFov;
			errorAngle = machineGun.noAimErrorAngle;
			walkSway.bobbingAmount = defaultBobbingAmount;
		}
		transform.localPosition = Vector3.Lerp(transform.localPosition, currentPosition, Time.deltaTime/aim.smoothTime);
		Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, currentFov, Time.deltaTime/aim.smoothTime);
	}
	
	void CameraRecoilDo(){
		Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, camPos, Time.deltaTime * cameraRecoil.smooth);
	}
	
	void RotationRealism (){
		float Xinput=Input.GetAxis("Mouse X");
		float Yinput=Input.GetAxis("Mouse Y");
		float currentAnglex = 0;
		float currentAngley = 0;
	
		if(Mathf.Abs(Xinput)>0.1f){
			if(Xinput < -0.1f){ 
				currentAngley = -rotationRealism.RotationAmplitude * Mathf.Abs(Xinput);	
			} 
			else if(Xinput > 0.1f){
				currentAngley = rotationRealism.RotationAmplitude *  Mathf.Abs(Xinput);
			} 
		} else { 
			currentAngley = 0;
		} 
		
		if(Mathf.Abs(Yinput)>0.1f){
			if(Yinput < -0.1f){ 
				currentAnglex = rotationRealism.RotationAmplitude * Mathf.Abs(Yinput);
			} 
			else if(Yinput > 0.1f){
				currentAnglex = -rotationRealism.RotationAmplitude *  Mathf.Abs(Yinput);
			} 
		} else { 
			currentAnglex = 0;
		} 
		
		Quaternion target = Quaternion.Euler (currentAnglex, currentAngley, 0);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * rotationRealism.smooth);
	}
	
	void SmoothMove (){
		float moveOnY = controller.velocity.y;
		float m = 0;
		float moveOnZ = -Input.GetAxis("Vertical");
		
		if(moveOnY > SmoothMovement.maxAmount+1)
			m = -SmoothMovement.maxAmount;
		
		if(moveOnY < -SmoothMovement.maxAmount-1)
			m = SmoothMovement.maxAmount;
		
		if (moveOnZ> SmoothMovement.maxAmount)
			moveOnZ = SmoothMovement.maxAmount;
		
		if (moveOnZ < -SmoothMovement.maxAmount)
			moveOnZ = -SmoothMovement.maxAmount;
		
		Vector3 NewGunPos = new Vector3 (transform.localPosition.x, transform.localPosition.y + m, transform.localPosition.z + moveOnZ);
		transform.localPosition = Vector3.Lerp (transform.localPosition, NewGunPos, Time.deltaTime * SmoothMovement.Smooth);
	}
	
	void SelectWeapon(){
		canFire = true;
		canAim = true;
		aimed = false;
		BroadcastMessage ("TakeIn");
	}
	
	void DeselectWeapon(){
		aimed = false;
		isReload = false;
		canFire = false;
		canAim = false;
		BroadcastMessage ("TakeOut");
	}
}
