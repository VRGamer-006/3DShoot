using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	public LayerMask mask = -1;
	public int speed = 500;
	public float life = 3;
	public int impactForce = 10;
	public bool impactHoles = true;
	public List<GameObject> impactObjects;
    private int damageAmount =20;
	private Vector3 velocity;
	private Vector3 newPos;
	private Vector3 oldPos;
	private bool hasHit = false;
	
	void Start () {
		newPos = transform.position;
		oldPos = newPos;
		velocity = speed * transform.forward;

		Destroy( gameObject, life );
	}
	
	void Update () {
		if( hasHit )
			return;
		newPos += velocity * Time.deltaTime;
		Vector3 direction = newPos - oldPos;
		float distance = direction.magnitude;
	
		if (distance > 0) {
			RaycastHit hit;
	
			if (Physics.Raycast(oldPos, direction, out hit, distance, mask)) {
				newPos = hit.point;
				hasHit = true;
				Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                
                Health targetHealth = hit.collider.GetComponent("Health") as Health;              
                if (targetHealth)
                {
                    int damage = (int)Random.Range(damageAmount / 2, damageAmount);
                    targetHealth.OnDamage(damage, -transform.forward, hit.point, 0);                   
                }
				if (hit.rigidbody){
					hit.rigidbody.AddForce( transform.forward * impactForce, ForceMode.Impulse );
				}
			
				if(impactHoles){
					for(int i = 0; i<impactObjects.Count; i++){
						if(hit.transform.tag == impactObjects[i].name){
							GameObject hole = (GameObject)Instantiate(impactObjects[i], hit.point, rotation);
							hole.transform.parent = hit.transform;
						}
					}
				}

				Destroy (gameObject, 1);
			}
		}
	
		oldPos = transform.position;
		transform.position = newPos;
	}
}
