using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour {

    public AudioClip walkSounds;
    public MonoBehaviour behaviourOnSpotted ;
    public MonoBehaviour behaviourOnLostTrack ;
    public Transform player;
    public Transform character;
    private SpiderAnimation spiderAnimation;
    public GameObject spiderObject;
    private Vector3 dir;
    public  Vector3 movementDirection = Vector3.zero;   
	private float walkingSpeed  = 1.5f;
    private float walkingSnappyness = 10;
    private float turningSmoothing = 0.3f;
    private BoxCollider boxCollider;
    private Vector3 boxSize;
    private bool shoot = false;
    private float lastTime;
    
    void Awake()
    {       
        spiderAnimation = spiderObject.GetComponent("SpiderAnimation") as SpiderAnimation;  
        player = GameObject.FindWithTag("Player").transform;
        dir = character.forward;
        boxCollider = GetComponent<BoxCollider>();
        boxSize = boxCollider.size;
    }
    void OnEnable()
    {
        lastTime = Time.time;
    }
    void FixedUpdate()
    {
        facePlayer();
        if (!shoot)
        {
            Vector3 targetVelocity = movementDirection * walkingSpeed;
            Vector3 dir = targetVelocity - character.rigidbody.velocity;
            character.rigidbody.AddForce(dir * walkingSnappyness, ForceMode.Impulse);
            if (Time.time > lastTime + 0.5f && movementDirection.sqrMagnitude > 1)
            {
                if (CameraAdaption.sound)
                {
                    audio.clip = walkSounds;
                    audio.Play();
                }   
                lastTime = Time.time;
            }
        }       
    }   
    void facePlayer()
    {      
        if(shoot)
        {
            movementDirection = player.position - character.position;
        }
        dir = Vector3.Slerp(dir, movementDirection, Time.deltaTime*100);
        dir.y = 0;
        character.rotation = Quaternion.LookRotation(dir);
    }
    static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        float angle = Vector3.Angle(dirA, dirB);

        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (shoot)
            {
                return;
            }
            OnSpotted1();
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player" || other.tag == "Enemy")
	    {
            OnLostTrack();
	    }
    }
    public void OnShoot()
    {
        shoot = true; 
        behaviourOnSpotted.enabled = false;
        behaviourOnLostTrack.enabled = false;
    }
    public void OnSpotted1()
    {       
        shoot = false;
        if (boxCollider)
        boxCollider.size = boxSize * 8.0f;
        if (spiderAnimation)
        	spiderAnimation.enemyRun();
        if (!behaviourOnSpotted.enabled) {              
	        behaviourOnSpotted.enabled = true;
	        behaviourOnLostTrack.enabled = false;		        
        }
    }
    public void OnLostTrack()
    {   
        shoot = false;
        if (boxCollider)
            boxCollider.size = boxSize;
        if (spiderAnimation)               
            spiderAnimation.enemyRun();   
        if (!behaviourOnLostTrack.enabled)
        {
            behaviourOnLostTrack.enabled = true;
            behaviourOnSpotted.enabled = false;
        }
    }
    public void OnAttack()
    {
        if (shoot)
        {
            return;
        }       
        if (spiderAnimation)
            spiderAnimation.enemyRun();
        if (!behaviourOnSpotted.enabled)
        {
            behaviourOnSpotted.enabled = true;
            behaviourOnLostTrack.enabled = false;
        }
    }
}
