using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Judges;

public class KeyInputManager : MonoBehaviour
{
    bool isLNPRESSED, isAutoPressed;
    RaycastHit2D hit;
    public GameObject judgeobj, lineEf, hitef;
    GameObject ef;
    GameObject LNEND;
    public Transform lnhitef;
    public Sprite DEF, PRESS;
    public Sprite DEF4K, PRESS4K;
    public int idx;
    float LNENDTime;
    int shiftInput;
    SpriteRenderer rend;
    MusicHandler player;
    NotePlayer reader;
    ScoreManager manager;

    // Start is called before the first frame update
    // Update is called once per frame
    private void Start()
    {
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        rend = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(0.74f * Manager.ColumnWidth, 0.6f);
        var g = GameObject.FindWithTag("NoteSys");
        reader = g.GetComponent<NotePlayer>();
        manager = g.GetComponent<ScoreManager>();


        if (Manager.keycount == 7)
            transform.position = new Vector2((idx - 3) * Manager.ColumnWidth, transform.position.y);
        else
        {
            
            DEF = DEF4K;
            PRESS = PRESS4K;
            rend.sprite = DEF;

            switch (idx)
            {
                case 1:
                    transform.position = new Vector2(-1.5f * Manager.ColumnWidth, transform.position.y);
                    break;
                case 2:
                    transform.position = new Vector2(-0.5f * Manager.ColumnWidth, transform.position.y);
                    transform.localScale = new Vector2(0.82f * Manager.ColumnWidth, 0.6f);
                    break;
                case 4:
                    transform.position = new Vector2(0.5f * Manager.ColumnWidth, transform.position.y);
                    transform.localScale = new Vector2(0.82f * Manager.ColumnWidth, 0.6f);
                    break;
                case 5:
                    transform.position = new Vector2(1.5f * Manager.ColumnWidth, transform.position.y);
                    break;
                default:
                    Destroy(gameObject);
                    break;
            }
        }
    }
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up);


        if (!Manager.AutoPlay)
        {

            if (Input.GetKey(KeySetting.keys[(KeyAction)idx]))
                rend.sprite = PRESS;
            else
                rend.sprite = DEF;
            if (Input.GetKeyDown(KeySetting.keys[(KeyAction)idx]))
                KeyInput(isLNPRESSED, true);
            if (Input.GetKeyUp(KeySetting.keys[(KeyAction)idx]))
                KeyRelease();
        }
        else
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y - .6f);
            RaycastHit2D auto = Physics2D.Raycast(pos, Vector2.up);
            if (isLNPRESSED)
            {
                if (LNENDTime - NotePlayer.Playback < 1f)
                    KeyRelease();
            }
            else if (auto)
            {
                ColNote note = auto.collider.GetComponent<ColNote>();
                if (note.getTime() - NotePlayer.Playback < 1f)
                {
                    KeyInput(isLNPRESSED, true);
                    if (!note.isLN())
                        KeyRelease();
                }
            }

        }

        if (isLNPRESSED) //롱놋 안 땠을 시 미스처리
        {
            float error = NotePlayer.Playback - LNENDTime;
            if (error > 270f)
            {
                ScoreManager.combo = 0;
                getJudge(4);
                isLNPRESSED = false;
            }
            lnhitef.gameObject.SetActive(true);
        }
        else lnhitef.gameObject.SetActive(false);




    }
    void KeyInput(bool LN, bool effect)
    {
        if (effect)
        {
            if (!ef)
            {
                ef = Instantiate(lineEf); //라인이펙트
                ef.GetComponent<LineEffects>().setCol(idx);
            }
            else
            {
                ef.GetComponent<LineEffects>().setActive();
            }
        }

        isLNPRESSED = false;
        float errorUp;
        float errorDown;
        float error;

        if (LN)
        {
            float e = NotePlayer.Playback - LNENDTime;
            error = Mathf.Abs(e);
            if (error < 180f)
                manager.AddError(e);
            cacLNJudge(error);
        }
        else
        {
            float eru = 0f, erd = 0f;
            #region if use raycast
            RaycastHit2D hitup, hitdown;
            hitup = Physics2D.Raycast(transform.position, Vector2.up); //위로 레이
            hitdown = Physics2D.Raycast(transform.position, Vector2.down); //아래로 레이
            if (hitup.collider != null)
            { //위 체크
                eru = NotePlayer.Playback - hitup.collider.GetComponent<ColNote>().getTime();
                errorUp = Mathf.Abs(eru); //현재시간에서 노트의 시간값 빼서 오차가져오기
            }
            else errorUp = 9999f;
            if (hitdown.collider != null)
            { //아래 체크
                erd = NotePlayer.Playback - hitdown.collider.GetComponent<ColNote>().getTime();
                errorDown = Mathf.Abs(erd);
            }
            else errorDown = 9999f;

            if (errorUp <= 174.4f || errorDown <= 174.4f)
            {
                RaycastHit2D CloseHit;
                ColNote cn;
                if (errorUp < errorDown)
                {
                    error = errorUp;
                    CloseHit = hitup;
                    manager.AddError(eru);
                    cn = CloseHit.collider.GetComponent<ColNote>();
                }
                else
                {
                    error = errorDown;
                    CloseHit = hitdown;
                    manager.AddError(erd);
                    cn = CloseHit.collider.GetComponent<ColNote>();
                }
                if (cn.isLN())
                {
                    isLNPRESSED = true;
                    hit = CloseHit;
                    cn.pressed = true;
                    LNENDTime = cn.getLnLength();
                }
                else
                {
                    createhitef();
                    cn.InsertQueue();
                    //Destroy(CloseHit.collider.gameObject);
                }

                int i = CloseHit.collider.GetComponent<ColNote>().KeySound;
                if (i != -1)
                    player.PlaySample(i);

                cacJudge(error, cn.isLN());
                
            }
            else if (hitup)
            {
                int i = hitup.collider.GetComponent<ColNote>().KeySound;
                if (i != -1)
                    player.PlaySample(i);
            }
            #endregion
        }
    }
    void createhitef()
    {
        Instantiate(hitef, transform.position, Quaternion.identity);
    }
    void KeyRelease()
    {
        ef.GetComponent<LineEffects>().delete();
        if (isLNPRESSED)
        {
            KeyInput(true, false);
            if (hit.collider != null)
                hit.collider.GetComponent<ColNote>().DestroyCollider();
        }
    }
    void cacJudge(float error, bool ln)
    {
        if (error <= Timings.j300k)
        {
            ScoreManager.combo++;
            getJudge(0);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error <= Timings.j300)
        {
            ScoreManager.combo++;
            getJudge(1);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error <= Timings.j200)
        {
            ScoreManager.combo++;
            getJudge(2);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error <= Timings.j100)
        {
            ScoreManager.combo++;
            if (ln)
                getJudge(3);
            else
                getJudge(6);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else
        {
            ScoreManager.combo = 0;
            if (ln)
                getJudge(4);
            else
                getJudge(7);
        }
    }
    void cacLNJudge(float error)
    {
        if (error <= LNTimings.j300k)
        {
            ScoreManager.combo++;
            getJudge(0);
        }
        else if (error <= LNTimings.j300)
        {
            ScoreManager.combo++;
            getJudge(1);
        }
        else if (error <= LNTimings.j200)
        {
            ScoreManager.combo++;
            getJudge(2);
        }
        else if (error <= LNTimings.j100)
        {
            ScoreManager.combo++;
            getJudge(3);
        }
        else
        {
            ScoreManager.combo = 0;
            getJudge(4);
        }
    }
    void getJudge(int idx)
    {
        judgeobj.SetActive(true);
        judgeobj.GetComponent<Judgement>().setInfo(idx);
    }
}
