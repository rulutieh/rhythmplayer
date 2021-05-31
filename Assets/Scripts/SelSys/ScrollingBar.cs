using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollingBar : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    FileLoader Loader;
    FileSelecter Select;
    RectTransform rect;
    float delay, pos;
    Vector2 MyPos, curPos;
    bool dragging;
    void Start()
    {
        Loader = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        Select = GameObject.FindWithTag("SelSys").GetComponent<FileSelecter>();
        rect = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        curPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        delay = 0;
        dragging = false;
    }

    void Update()
    {
        if (dragging)
        {
            pos = curPos.y - MyPos.y;
            if (pos > 200f) pos = 200f;
            if (pos < -200f) pos = -200f;
            var length = Mathf.Abs(pos);
            if (length > 1f)
            {
                delay += length * Time.deltaTime;
            }
        }
        if (delay > 2.8f)
        {
            if (pos < 0.3f)
            {
                if (GlobalSettings.decide < Loader.list.Count - 1)
                {
                    GlobalSettings.decide++;
                    Select.SongScroll();
                }
            }
            if (pos > 0.3f)
            {
                if (GlobalSettings.decide > 0)
                {
                    GlobalSettings.decide--;
                    Select.SongScroll();
                }
            }
            delay = 0;
            MyPos = rect.position;
        }
    }


}
