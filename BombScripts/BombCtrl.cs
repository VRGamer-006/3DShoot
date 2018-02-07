using UnityEngine;
using System.Collections;

public class BombCtrl : MonoBehaviour {
	public Transform bomb;
	public Transform bombA;
	public Transform bombB;
	public float blink = 1.0f;
	private float timeLevel = 0.5f;
	private float lastBlinkTime = 0;
	public AudioClip bombWarningClip;
	public TimeDisplay currentTime;
	private bool isExplode;
	public GameObject explodePrefab;
    private ParticleEmitter explodeEffect;
	public AudioClip explodeAudioClip;
	private MeshRenderer bombMeshRenderer;
	void OnWillRenderObject () {
		renderer.sharedMaterial.SetFloat ("_SelfIllumStrength", blink);
	}
	void Start () {        
		bombMeshRenderer = GetComponent<MeshRenderer>();
		if((int)(Random.value*1.999f) == 0) {
			bomb.position = bombA.position;
		}
		else {
			bomb.position = bombB.position;
		}
		
	}
	void Update () {
		if(isExplode) {
			return;
		}
		timeLevel = 1 - currentTime.GetTime() / currentTime.totalTime;
		float deltaBlink = 1 / Mathf.Lerp (2, 15, timeLevel);
		if (Time.time > lastBlinkTime + deltaBlink) {
			lastBlinkTime = Time.time;
			Blink();
            if (CameraAdaption.sound)
            {
                audio.clip = bombWarningClip;
                audio.Play();
            }
          
		}
	}
	
	void Blink () {
		blink = 1.0f - blink;
	}
	
	void OnExplode () {
		isExplode = true;
        GameObject exploEffect = Instantiate(explodePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        exploEffect.transform.parent = this.transform;
        exploEffect.transform.localPosition = Vector3.zero;
        explodeEffect = exploEffect.particleEmitter;
        if (CameraAdaption.sound)
        {
            audio.clip = explodeAudioClip;
            audio.Play();
        }
		explodeEffect.Emit();
		
		bombMeshRenderer.enabled = false;
		
		Destroy(gameObject,3.74f);
	}
}
