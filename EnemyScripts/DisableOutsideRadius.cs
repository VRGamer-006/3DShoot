using UnityEngine;
using System.Collections;

public class DisableOutsideRadius : MonoBehaviour {
    public GameObject aiMoveObj;
    public GameObject target;
    private BoxCollider boxCollider;
    private float activeRadius;  
    private SpiderAnimation spiderAnimation;
    private Vector3 boxSize;
	void Awake () {           
        boxCollider = GetComponent<BoxCollider>();
        boxSize = boxCollider.size;
        Disable();	
	}
    void OnTriggerEnter(Collider other)
    {       
        if(other.tag =="Player")
        {      
            Enable();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {          
            Disable();
        }
    }
    void Disable()
    {
        if (target.activeSelf && aiMoveObj.activeSelf)
        {
            aiMoveObj.SendMessage("OnLostTrack"); 
        }
        target.SetActive(false);
        boxCollider.size = boxSize;
    }
    void Enable()
    {        
        target.SetActive(true);       
        boxCollider.size = boxSize * 1.5f;
    }

}
