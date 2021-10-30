using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRenderer : MonoBehaviour
{
    public GameObject lnend, lntemp;
    GameObject inst;
    NotePlayer rdr;
    public Sprite dk, sp, dkln, spln, def, defspr;
    public bool lncreated;
    SpriteRenderer rend, rend2;
    Sprite tempspr;
    int COLUMN;
    int TIME;
    float LNLENGTH, cf, NoteTiming;
    bool ISLN;

    void Awake()
    {
        var sys = GameObject.FindWithTag("NoteSys");
        rdr = sys.GetComponent<NotePlayer>();
        rend = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(transform.localScale.x * Manager.ColumnWidth * 0.98f, transform.localScale.y);
        if (Manager.isCutOff) cf = 0.2f; else cf = 0;

    }
    void OnEnable()
    {
        lncreated = false;
        lnend = null;
    }


    void Update()
    {
        if (ISLN)
        {
            if (!lnend)
            {
                if (!lncreated) //롱노트 끝이 로드가 되지 않았을 때 미리 롱노트를 출력
                {
                    lncreated = true; //중복생성 방지
                    inst = Instantiate(lntemp, transform.position, Quaternion.identity);
                    inst.transform.parent = gameObject.transform;
                    rend2 = inst.GetComponent<SpriteRenderer>();
                    float height = inst.transform.GetComponent<SpriteRenderer>().bounds.size.y;
                    float gap = 999f - transform.position.y;
                    rend2.sprite = tempspr;
                    inst.transform.localScale = new Vector2(1f, gap / height);
                }
            }
            else
            {
                if (inst)
                {
                    inst.transform.position = transform.position;
                    float height = transform.GetComponent<SpriteRenderer>().bounds.size.y;
                    float gap = lnend.transform.position.y - transform.position.y - cf;
                    if (gap < 0) gap = 0;
                    inst.transform.localScale = new Vector2(1f, gap / height);
                }
                if (LNLENGTH + 200 < NotePlayer.Playback)
                {
                    Destroy(lnend);
                    Destroy(gameObject);
                }
            }
        }
        else if (TIME + 178.4f < NotePlayer.Playback)
        {
            InsertQueue();
        }
    }
    private void LateUpdate()
    {
        transform.position = new Vector2(transform.position.x, (float)(NotePlayer.judgeoffset + (NoteTiming - NotePlayer.PlaybackChanged) * NotePlayer.multiply));
    }
    public void SetInfo(int c, int t, bool ln, float length, float nt)
    {
        COLUMN = c;
        TIME = t; ISLN = ln; LNLENGTH = length; NoteTiming = nt;
        if (Manager.keycount == 7)
        {
            transform.position = new Vector2((c - 3f) * Manager.ColumnWidth, transform.position.y);

            switch (c)
            {
                case 1:
                    rend.sprite = dk;
                    tempspr = dkln;
                    break;
                case 3:
                    rend.sprite = sp;
                    tempspr = spln;
                    break;
                case 5:
                    rend.sprite = dk;
                    tempspr = dkln;
                    break;

                default:
                    rend.sprite = def;
                    tempspr = defspr;
                    break;
            }
        }
        else
        {
            tempspr = defspr;
            transform.position = new Vector2((c - 1.5f) * Manager.ColumnWidth, transform.position.y);
            switch (c)
            {
                case 1:
                case 2:
                    rend.sprite = dk;
                    tempspr = dkln;
                    break;

                default:
                    rend.sprite = def;
                    tempspr = defspr;
                    break;
            }
        }

    }
    public void setLnEnd(GameObject lne)
    {
        lnend = lne;
    }
    public void InsertQueue()
    {
        if (inst) Destroy(inst);
        transform.position = new Vector2(0, 1000f);
        rdr.start_queue.Enqueue(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
