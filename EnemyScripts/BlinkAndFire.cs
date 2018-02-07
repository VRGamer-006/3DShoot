using UnityEngine;
using System.Collections;

public class BlinkAndFire : MonoBehaviour
{
    public GameObject enemy;
    public GameObject playerBody;
    public GameObject bulletPrefab;
    public ParticleSystem pSystem;
    private float lastFireTime;
    public AudioClip fireSound;
    bool firing = true;
    void OnEnable()
    {   
        lastFireTime = Time.time;        
    }
	void Update () {
        pSystem.startColor = Color.Lerp(Color.red, Color.red / 2, Mathf.PingPong(Time.time, 1));
        if (firing)
        {
            if (Time.time > lastFireTime + 0.5f)
            {
                lastFireTime = Time.time;
                if (CameraAdaption.sound && fireSound)
                {
                    audio.clip = fireSound;
                    audio.Play();
                }         
                Instantiate(bulletPrefab, transform.position, transform.rotation); 
            }
        }
	}   
}
