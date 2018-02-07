using UnityEngine;
using System.Collections;

public class WeapenPageControl : MonoBehaviour {

    enum WeapenName
    {
        Deagle,
        SniperRifle,
        M87T,
        MP5KA4,
        STW_25
    }
    public UILabel moneyLabel;
    public UILabel pageLabel;
    public GameObject[] weapens;
    public int totalPages;
    public int curPage;
    void OnEnable()
    { 	
        if (pageLabel)
        {
            pageLabel.text = "1";
        }    
        curPage = 1;
        float money = Mathf.Clamp(PlayerPrefs.GetFloat("money",300000f), 0, 99999);
        moneyLabel.text = money.ToString();
        for (int i = 0; i < weapens.Length; i++)
        {
            weapens[i].SetActive(i < 4);
        } 
    }

    void PageDown()
    {        
        int page = curPage;
        curPage++;
        curPage = Mathf.Min(curPage,totalPages);
        if (page == curPage)
        {
            return;
        }
        else
        {
            int end = Mathf.Min(4 * curPage, weapens.Length);
            int start = 4 * (page - 1);
            for (int i = start; i < end; i++)
            {             
                weapens[i].SetActive(i >= 4 * page);                            
            }         
        }
        if (pageLabel)
        {
            pageLabel.text = curPage.ToString();
        }
       
    }    
    void PageUp()
    {
        int page = curPage;
        curPage--;
        curPage = Mathf.Max(curPage,1);
        if (page == curPage)
        {
            return;
        }
        else
        {
            int start = 4 * (curPage - 1);
            int end = Mathf.Min(4 * page, weapens.Length);
            for (int i = start; i < end; i++)
            {
                weapens[i].SetActive(i < 4 * curPage);                          
            }
        }
        if (pageLabel)
        {
            pageLabel.text = curPage.ToString();
        }
    }
}
