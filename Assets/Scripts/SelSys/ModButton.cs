using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModButton : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver;
    public GameObject sys, rank;
    FileSelecter select;
    Manager setting;

    public int mod;
    float scroll;
    // Start is called before the first frame update
    void Start()
    {
        if (!sys)
            sys = GameObject.FindWithTag("SelSys");
        select = sys.GetComponent<FileSelecter>();
        setting = GameObject.FindWithTag("world").GetComponent<Manager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(mod)
        {
            case 0:
                select.SetSuffle();
                break;
            case 1:
                select.SetSort();
                break;
            case 3:
                select.SetSpecial();
                break;
            case 5:
                rank.GetComponent<RankPanel>().SwitchOnline();
                break;

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (mod == 2)
        {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            if (isOver)
            {
                if (Manager.scrollSpeed < 4f) //스크롤 업
                    if (scroll > -0.001f)
                    {
                        Manager.scrollSpeed += 0.1f;
                        setting.SaveSelection();
                    }
                if (Manager.scrollSpeed > 1f) //스크롤 다운
                    if (scroll < 0.001f)
                    {
                        Manager.scrollSpeed -= 0.1f;
                        setting.SaveSelection();
                    }
            }
        }
    }
}
