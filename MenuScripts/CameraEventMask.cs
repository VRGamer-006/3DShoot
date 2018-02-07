using UnityEngine;
using System.Collections;

public class CameraEventMask : MonoBehaviour {
    public LayerMask mainMask;
    public LayerMask otherMask;
    public UICamera uiCamera;
	public GameObject soundOnBtn;
	public GameObject soundOffBtn;
	AudioListener audioListener;
	void Awake()
	{		
		audioListener = GetComponent<AudioListener>();
		int soundflag =PlayerPrefs.GetInt("sound",1);
		soundOnBtn.SetActive(soundflag == 1);
		soundOffBtn.SetActive(soundflag == 0);
		audioListener.enabled = (soundflag == 1);
	}
	void SoundOn_Btn()
	{
		PlayerPrefs.SetInt("sound",0);
		audioListener.enabled = false;		
		soundOffBtn.SetActive(true);
		soundOnBtn.SetActive(false);		
	}
	void SoundOff_Btn()
	{
		PlayerPrefs.SetInt("sound",1);
		audioListener.enabled = true;
		soundOnBtn.SetActive(true);
		soundOffBtn.SetActive(false);		
	}
    void SetOtherMask()
    {
        uiCamera.eventReceiverMask = otherMask.value;       
    }
    void SetMainMask()
    {
        uiCamera.eventReceiverMask = mainMask.value;
    }
    void ExitGame()
    {
        Application.Quit();
    }
}
