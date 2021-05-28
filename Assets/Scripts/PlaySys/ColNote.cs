using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColNote : MonoBehaviour
{
    public bool pressed, lnsetpressed;
    public int KeySound;
    //FileReader
    ScoreManager scm;
    FileReader rdr;
    public BoxCollider2D box;
    int COLUMN;
    int TIME;
    float LNLENGTH, _TIME;
    bool ISLN;
    void Awake()
    {

        var sys = GameObject.FindWithTag("NoteSys");
        scm = sys.GetComponent<ScoreManager>();
        rdr = sys.GetComponent<FileReader>();
        box = GetComponent<BoxCollider2D>();
    }
    void OnEnable()
    {
        box.enabled = true;
        pressed = false;
        lnsetpressed = false;
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
                    scm.SetJudge(1);
                    InsertQueue();
                    //Destroy(gameObject);
                }
                else
                if (LNLENGTH + 200f < FileReader.Playback)
                {
                    InsertQueue();
                    //Destroy(gameObject);
                }
            }
        }
        else if (TIME + 178.4f < FileReader.Playback)
        {
            scm.SetJudge(0);
            InsertQueue();
            //Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {

        transform.position = new Vector2(transform.position.x, (float)
            (FileReader.judgeoffset + (_TIME - FileReader.Playback) * 0.01f));
    }

    public void SetInfo(int c, int t, bool ln, float length, float nt, int ksidx)
    {
        transform.localScale = new Vector2(transform.localScale.x * GlobalSettings.ColWidth / 0.85f, transform.localScale.y);
        TIME = t; ISLN = ln; LNLENGTH = length; _TIME = nt;
        transform.position = new Vector2((c - 3f) * GlobalSettings.ColWidth, transform.position.y);
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
    public void InsertQueue()
    {
        transform.position = new Vector2(0, 1000f);
        rdr.n_queue.Enqueue(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
