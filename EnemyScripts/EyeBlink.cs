using UnityEngine;
using System.Collections;

public class EyeBlink : MonoBehaviour {

    float lastBlinkTime;
    float blink = 1;
    void Awake()
    {
        lastBlinkTime = Time.time;
    }
	
	void Update () {
        if (Time.time - lastBlinkTime >= 1)
        {
            lastBlinkTime = Time.time;
            blink = 1 - blink;
            gameObject.renderer.material.SetColor("_Color", new Color(1,0,0,blink));
        }      
	}
}
