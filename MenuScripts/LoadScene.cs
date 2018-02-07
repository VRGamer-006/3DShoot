using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
	public GameObject showObj;
	public GameObject hideObj; 
	AsyncOperation asyncOperation;
	
	private UILabel loadingLabel;
    public Transform progressBar;
    private string[] loadTxt = { "加载中。  ", "加载中。。 ", "加载中。。。" };
	private float count = 0;
	private float currentTime;
	
	void Start () {
        loadingLabel = showObj.GetComponent<UILabel>();
	}
	
	void LoadGameScene() {
        hideObj.SetActive(false);
        showObj.SetActive(true);
        StartCoroutine(LoadingAnimation());
    }
	
	IEnumerator LoadingAnimation() {
		yield return new WaitForSeconds(0.1f);
        asyncOperation = Application.LoadLevelAsync("GameScene");
        while (!asyncOperation.isDone) {     
			int index = (int)count % 3;
            loadingLabel.text = loadTxt[index];
            count += Time.deltaTime * 3;
            Vector3 locationPosition = progressBar.localPosition;
            locationPosition.x += Time.deltaTime * 500;
            locationPosition.x = (locationPosition.x > 200) ? -200 : locationPosition.x;
            progressBar.localPosition = locationPosition;
            yield return new WaitForSeconds(0.01f);
		}       
	}
}
