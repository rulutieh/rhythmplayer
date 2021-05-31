using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SelectIcon : MonoBehaviour , IPointerClickHandler
{
    FileSelecter select;
    RectTransform rect;
    Image image;
    public int idx;
    public int diffcount;
    // Start is called before the first frame update
    void Start()
    {
        GameObject sel = GameObject.FindWithTag("SelSys");
        if (!sel) Destroy(gameObject);
        select = sel.GetComponent<FileSelecter>();
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalSettings.diffselection == idx)
        {
            image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            image.color = new Color(1, 1, 1, 0.4f);
        }
        rect.anchoredPosition = new Vector2(-171.9f - diffcount * 20f / 2f + 20f * idx, 135.2f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (select.DiffChangeEnable())
        {
            GlobalSettings.diffselection = idx;
            select.getDiffinfo();
        }
    }
}
