using UnityEngine;
using System.Collections;

public class WaitForDestroy : MonoBehaviour {
	public float lifeTime = 0.3f;
	public bool isFade = false;
	public float duration = 0.01f;
		
	void Awake () {
		if(!isFade) {
			Destroy(gameObject, lifeTime);
		}
		else {
			StartCoroutine(FadeAndDestroy());
		}
	}
	
	IEnumerator FadeAndDestroy () {
		while(true) {
			yield return new WaitForSeconds(lifeTime);
			Color c = gameObject.renderer.material.GetColor("_Color");
			c.a = Mathf.Lerp(c.a, 0.5f, duration);
			gameObject.renderer.material.color = c;
			if(c.a < 0.52f) {
				Destroy(gameObject);
			}
		}
	}
}
