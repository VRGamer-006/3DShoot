using UnityEngine;
using System.Collections;

public class SpiderAnimation : MonoBehaviour
{
    public AnimationClip idle;
    public AnimationClip jump;
    public AnimationClip run;
    public AnimationClip shoot;
    public static float shootWeight = 0;
    public static float Speed = 1;
    public GameObject gun_muzzle_Object;
	void OnEnable () {
        animation[idle.name].enabled = true;
        animation[idle.name].layer = 1;
        animation[idle.name].speed = 2;

        animation[run.name].enabled = true;     
        animation[run.name].layer = 1;
	
        animation[shoot.name].enabled = true;   
        animation[shoot.name].layer = 1;
		animation[shoot.name].speed = 0.5f;
        enemyIdle();   
	
	}
    public void enemyShoot()
    {
        gun_muzzle_Object.SetActive(true);
        animation.CrossFade(shoot.name,0.2f); 	
		animation[shoot.name].speed = 0.5f;
    }
    public void enemyRun()
    {
        gun_muzzle_Object.SetActive(false);
        animation.CrossFade(run.name,0.2f);
		animation[run.name].layer = 1;
		animation[run.name].speed = 4.5f;
    }
    public void enemyIdle()
    {      
        gun_muzzle_Object.SetActive(false);
        animation.CrossFade(idle.name, 1f);
		animation[idle.name].speed = 2;
    }
}
