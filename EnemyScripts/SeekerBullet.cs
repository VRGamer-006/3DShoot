using UnityEngine;
using System.Collections;

public class SeekerBullet : MonoBehaviour {
public float speed = 20.0f;
public float lifeTime = 1.5f;
public int damageAmount = 20;
public float forceAmount  = 5;
public float radius  =0.1f;
public float seekPrecision  = 1.3f;
public LayerMask ignoreLayers;
public float noise = 0.2f;
public GameObject explosionPrefab;
private Vector3 dir;
private float spawnTime;
private GameObject targetObject;
private Transform tr;
bool collided = false;
private float sideBias ;
public AudioClip destroyedSound;
void OnEnable () {
	tr = transform;
	dir = transform.forward;
    targetObject = GameObject.FindWithTag("Player");
	spawnTime = Time.time;
	sideBias = Mathf.Sin (Time.time * 5);
}	
	void Update () {
        if (collided)
        {
            return;
        }
        if(targetObject)
        {
            Vector3 targetPos = targetObject.transform.position;
            targetPos = targetPos + transform.right * (Mathf.PingPong(Time.time, 1.0f) - 0.5f) * noise;       
            Vector3 targetDir = targetPos - tr.position;
            float targetDist = targetDir.magnitude;
            targetDir = targetDir / targetDist;
            tr.position = tr.position + (dir * speed) * Time.deltaTime;
        }
        Collider[] hits = Physics.OverlapSphere(tr.position, radius, ~ignoreLayers.value);     
        foreach (Collider hit in hits)
        {   
            if(hit.isTrigger)
            {
                continue;
            }
            Health targetHealth = hit.GetComponent("Health") as Health;       
            if(hit.rigidbody)
            {               
                Vector3 force = tr.forward * forceAmount;
                force.y = 0;
                hit.rigidbody.AddForce(force,ForceMode.Impulse);
            }
            if (targetHealth)
            {
                int damage = (int)Random.Range(damageAmount/2,damageAmount);
                targetHealth.OnDamage(damage, -tr.forward, tr.position, 0.5f);
            }
            collided = true;
        }
        if(collided)
        {
            if (CameraAdaption.sound && destroyedSound)
            {
                audio.clip = destroyedSound;
                audio.Play();
            }
            Instantiate(explosionPrefab, tr.position, tr.rotation);
            Destroy(gameObject, 2);
            gameObject.renderer.enabled = false;            
        }
	}
}
