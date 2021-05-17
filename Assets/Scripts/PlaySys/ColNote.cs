using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColNote : MonoBehaviour
{
    public bool pressed, lncreated, lnsetpressed;
    public int KeySound;
    ScoreManager reader;
    public BoxCollider2D box;
    int COLUMN;
    int TIME;
    float LNLENGTH, _TIME;
    bool ISLN;
    void Awake()
    {
        reader = GameObject.FindWithTag("NoteSys").GetComponent<ScoreManager>();
        box = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x - 0.3f, transform.position.y), new Vector2(transform.position.x + 0.3f, transform.position.y),Color.red);
        if (ISLN)
        {
            if (TIME + 178.4f < FileReader.Playback)
            {
                if (!pressed)
                {
                    reader.SetJudge(1);
                    Destroy(gameObject);
                }
                else
                if (LNLENGTH + 200f < FileReader.Playback)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (TIME + 178.4f < FileReader.Playback)
        {
            reader.SetJudge(0);
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {

        transform.position = new Vector2(transform.position.x, (float)
            (FileReader.judgeoffset + (_TIME - FileReader.Playback) * 0.01f));
    }

    public void SetInfo(int c, int t, bool ln, float length, float nt, int ksidx)
    {
        transform.localScale = new Vector2(transform.localScale.x * scrSetting.ColWidth / 0.85f, transform.localScale.y);
        TIME = t; ISLN = ln; LNLENGTH = length; _TIME = nt;
        transform.position = new Vector2((c - 3f) * scrSetting.ColWidth, transform.position.y);
        KeySound = ksidx;

    }
    public float getTime()
    {
        return TIME;
    }
    public bool isLN()
    {
        return ISLN;
    }
    public float getLnLength()
    {
        return LNLENGTH;
    }
    public void DestroyCollider()
    {
        if (box) box.enabled = false;
    }
    public void setPressed()
    {
        lnsetpressed = true;
    }
}
