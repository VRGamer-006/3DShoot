using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {
	public float updateInterval = 0.5F;
	private double lastInterval;
	private int frames = 0;
	private float fps;
	
	private GameObject[] fpsObj;
	
	void Start() {
		fpsObj = GameObject.FindGameObjectsWithTag("FPS");
		for(int i=0;i<fpsObj.Length;i++) {
			if(i==0) continue;
			Destroy(fpsObj[i]);
		}
		DontDestroyOnLoad(this.gameObject);
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }
	void OnGUI() {
		GUI.Label(new Rect(Screen.width-40, Screen.height-20, 80, 20),"" + fps.ToString("f2"));
	}
	void Update() {
	    ++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval) {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
	}
}
