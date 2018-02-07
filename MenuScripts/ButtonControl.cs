using UnityEngine;
using System.Collections;

public class ButtonControl : MonoBehaviour {
    enum WeapenName
    {
        Deagle,
        SniperRifle,
        M87T,
        MP5KA4,
        STW_25
    }
    public GameObject[] equipBtn;
    public GameObject[] furnishedBtn;
    void SetButtons()
    {
        for (int i = 0; i < furnishedBtn.Length; i++)
        {       
            if (furnishedBtn[i].activeSelf)
            {
                equipBtn[i].SetActive(true);
                if (!equipBtn[i].transform.parent.gameObject.activeSelf)
                {
                    equipBtn[i].transform.parent.gameObject.SetActive(true);
                    equipBtn[i].transform.parent.SendMessage("FurnishedBt_Click");
                    equipBtn[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    equipBtn[i].transform.parent.SendMessage("FurnishedBt_Click");
                }
                furnishedBtn[i].SetActive(false);

            }            
        }      
    }
}
