using UnityEngine;
using System.Collections;

public class ButtonTest : MonoBehaviour { 
    public GameObject disTargets;
    public GameObject enaTargets;
    public GameObject parentObj;
    public UILabel moneyLabel;
    public float price;
    void SetTarget()
    {  
        disTargets.SetActive(false);
        enaTargets.SetActive(true);       
    }
    void SetTarget_Buy_btn()
    {
        float money = PlayerPrefs.GetFloat("money", 300000f);
        if (money >= price)
        {
            PlayerPrefs.SetFloat("money", money - price);
            moneyLabel.text = (money - price).ToString();
            if (parentObj.activeSelf)
            {
                parentObj.SendMessage("BuyBt_Click");
            }
            disTargets.SetActive(false);
            enaTargets.SetActive(true);
        }       
    }
}
