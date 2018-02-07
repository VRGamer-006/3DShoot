using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public AudioClip deadClip;
    public GameObject robot;
    public GameObject aiMoveObj;
    private int maxHealth = 100;
    private int health;
    public float regenerateSpeed = 0.0f;
    public bool dead = false;
    public GameObject damagePrefab;

	public ProgressBloodBar bloodBar;
	
    private Transform damageEffectTransform;
    private ParticleEmitter damageEffect;
    public GameObject explorPrefab;
    private ParticleEmitter explorEffect;

	void Awake () {
		if(gameObject.tag == "Enemy") {
			maxHealth = PlayerPrefs.GetInt("enemyblood", 150);
		}
		
		health = maxHealth;
	    if(damagePrefab)
        {
            if (damageEffectTransform == null)
            {
                damageEffectTransform = transform;
            }
            GameObject effect = Instantiate(damagePrefab,Vector3.zero,Quaternion.identity) as GameObject;
            effect.transform.parent = damageEffectTransform;
            effect.transform.localPosition = Vector3.zero;
            damageEffect = effect.particleEmitter;

            if (gameObject.tag == "Enemy")
            {
                GameObject exploEffect = Instantiate(explorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                exploEffect.transform.parent = damageEffectTransform;
                exploEffect.transform.localPosition = Vector3.zero;
                explorEffect = exploEffect.particleEmitter;  
            }         
        }
	}

    public void OnDamage(int amount, Vector3 fromDirection,Vector3 damagePosition,float yOffer)
    {
        if (aiMoveObj)
        {
            aiMoveObj.SendMessage("OnAttack");
        }
		
        if (dead)
        {
            return;
        }
        health -= amount;
		if(transform.tag == "Player") {
			bloodBar.CalLastBlood(amount);
		}
        if(damageEffect)
        {
            damageEffect.transform.rotation = Quaternion.LookRotation(fromDirection,Vector3.up);
            damagePosition.y += yOffer;
            damageEffect.transform.position = damagePosition - fromDirection.normalized/2;   
            damageEffect.Emit();
        }
        if(health <= 0)
        {
            dead = true;
            if(transform.tag == "Player")
            {
                gameObject.collider.enabled = false;
            }
            if (transform.tag == "Enemy")
            {
                if (explorEffect)
                {
                    explorEffect.transform.position = transform.position;
                    explorEffect.Emit();
                }
                robot.SetActive(false);
                Destroy(gameObject.transform.parent.gameObject,1f);
            }
			if(deadClip && CameraAdaption.sound) {
                audio.clip = deadClip;
                audio.Play();
			}
        }
    }
}
