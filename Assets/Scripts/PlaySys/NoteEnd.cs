using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEnd : Note
{
    float TIME;
    public Sprite dk, sp, def;
    SpriteRenderer rend;
    NewInputSystem observ;
    private void Awake()
    {
        NoteInit();
        rend = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(transform.localScale.x * Manager.ColumnWidth * 0.98f, transform.localScale.y);
        if (Manager.isCutOff) rend.enabled = false;
        
    }
    private void OnEnable()
    {
        NoteAppear();
    }

    public void setInfo(int idx, int c, float t, GameObject obj, float nt) //콜룸, 타임, 시작노트
    {
        Index = idx;
        if (!Manager.isCutOff)
            if (Manager.keycount == 7)
            {
                if (c == 1 || c == 5) { rend.sprite = dk; } else if (c == 3) { rend.sprite = sp; } else { rend.sprite = def; }
            }
        else
            {
                if (c == 1 || c == 2) { rend.sprite = dk; } else { rend.sprite = def; }
            }
        TIME = t; NoteTiming = nt;
        if (obj)
            transform.position = new Vector2(obj.transform.position.x, transform.position.y);
        else
        {
            InsertQueue();
        }
        NoteTiming = nt;
    }

    public void InsertQueue()
    {
        transform.position = new Vector2(0, 1000f);
        rdr.end_queue.Enqueue(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
