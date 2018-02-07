using UnityEngine;
using System.Collections;

public class SliderTest : MonoBehaviour {
    public enum Widget
    {
        time,
        enemyblood
    }
    public Widget curWidge;
    public UISlider Slider;
    public UILabel  Lable;
    void OnEnable()
    {	
		switch(curWidge)
		{
			case Widget.time:
				float timeValue = PlayerPrefs.GetInt(curWidge.ToString(), 450);
    			Slider.value = (timeValue - 300)/300;
				Lable.text = timeValue.ToString();
			break;
			case Widget.enemyblood:
				float enemybloodValue = PlayerPrefs.GetInt(curWidge.ToString(), 150);
    			Slider.value = (enemybloodValue - 100)/100;
				Lable.text = enemybloodValue.ToString();
			break;			
		}
       
    }
    public void OnSliderChange() {
        float value = Slider.value;
		switch(curWidge)
		{
			case Widget.time:
				PlayerPrefs.SetInt(curWidge.ToString(), (int)(value * 300) + 300);		
				Lable.text = (PlayerPrefs.GetInt(curWidge.ToString())).ToString();
			break;
			case Widget.enemyblood:
				PlayerPrefs.SetInt(curWidge.ToString(), (int)(value * 100) + 100);
				Lable.text = (PlayerPrefs.GetInt(curWidge.ToString())).ToString();
			break;			
		} 	
    }
}
