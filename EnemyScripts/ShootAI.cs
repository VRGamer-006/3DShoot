using UnityEngine;
using System.Collections;

public class ShootAI : MonoBehaviour
{
    public GameObject aiMoveOjb;
    SpiderAnimation spa;
    Transform character;
    private Transform player;
    public Transform spaObject;
    private BoxCollider boxCollider;
    private Vector3 boxSize;
    private Vector3 dir;
    void Awake()
    {      
        spa = spaObject.GetComponent("SpiderAnimation") as SpiderAnimation;
        boxCollider = GetComponent<BoxCollider>();
        boxSize = boxCollider.size;
        character = transform;
        player = GameObject.FindWithTag("Player").transform;
        dir = transform.parent.forward;
	}	
     void OnTriggerEnter(Collider other)
     {
         if (other.tag == "Player")
         {
             spa.enemyShoot();
             boxCollider.size =  boxSize * 1.5f;
             aiMoveOjb.SendMessage("OnShoot");
         }
     }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            spa.enemyRun();
            boxCollider.size = boxSize;
            aiMoveOjb.SendMessage("OnSpotted1");
        }
    }	
}
