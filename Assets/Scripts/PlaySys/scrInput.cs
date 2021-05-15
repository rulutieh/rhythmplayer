using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrInput : MonoBehaviour
{
    bool isLNPRESSED, isAutoPressed;
    RaycastHit2D hit;
    public GameObject judgeobj, lineEf, hitef;
    GameObject ef;
    GameObject LNEND;
    public Transform lnhitef;
    public Sprite DEF, PRESS;
    public int idx;
    float LNENDTime;
    SpriteRenderer rend;

    FileReader reader;
    ScoreManager manager;

    // Start is called before the first frame update
    // Update is called once per frame
    private void Start()
    {

        rend = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(0.74f * scrSetting.ColWidth, 0.6f);
        var g = GameObject.FindWithTag("NoteSys");
        reader = g.GetComponent<FileReader>();
        manager = g.GetComponent<ScoreManager>();

    }
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up);


        if (!scrSetting.AutoPlay)
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
            Vector2 pos = new Vector2(transform.position.x, transform.position.y - 1f);
            RaycastHit2D auto = Physics2D.Raycast(pos, Vector2.up);
            if (isLNPRESSED)
            {
                if (LNENDTime - FileReader.Playback < 1f)
                    KeyRelease();
            }
            else if (auto)
            {
                ColNote note = auto.collider.GetComponent<ColNote>();
                if (note.getTime() - FileReader.Playback < 1f)
                {
                    KeyInput(isLNPRESSED, true);
                    if (!note.isLN())
                        KeyRelease();
                }
            }

        }

        if (isLNPRESSED) //롱놋 안 땠을 시 미스처리
        {
            float error = FileReader.Playback - LNENDTime;
            if (error > 270f)
            {
                ScoreManager.combo = 0;
                getJudge(4);
                isLNPRESSED = false;
            }
            lnhitef.gameObject.SetActive(true);
        }
        else lnhitef.gameObject.SetActive(false);

        transform.position = new Vector2((idx - 3) * scrSetting.ColWidth, transform.position.y);


    }
    void KeyInput(bool LN, bool effect)
    {
        if (effect)
        {
            if (!ef)
            {
                ef = Instantiate(lineEf); //라인이펙트
                ef.GetComponent<scrLineEf>().setCol(idx);
            }
            else
            {
                ef.GetComponent<scrLineEf>().setActive();
            }
        }

        isLNPRESSED = false;
        float errorUp;
        float errorDown;
        float error;

        float notetiming;
        if (LN)
        {
            float e = FileReader.Playback - LNENDTime;
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
                eru = FileReader.Playback - hitup.collider.GetComponent<ColNote>().getTime();
                errorUp = Mathf.Abs(eru); //현재시간에서 노트의 시간값 빼서 오차가져오기
            }
            else errorUp = 9999f;
            if (hitdown.collider != null)
            { //아래 체크
                erd = FileReader.Playback - hitdown.collider.GetComponent<ColNote>().getTime();
                errorDown = Mathf.Abs(erd);
            }
            else errorDown = 9999f;

            if (errorUp <= 174.4f || errorDown <= 174.4f)
            {
                if (errorUp < errorDown)
                {
                    error = errorUp;
                    notetiming = hitup.collider.gameObject.GetComponent<ColNote>().getTime(); //가까운 노트의 목표타이밍값 가져오기
                    manager.AddError(eru);
                    if (hitup.collider.gameObject.GetComponent<ColNote>().isLN())
                    {
                        isLNPRESSED = true;
                        hit = hitup;
                        hit.collider.GetComponent<ColNote>().pressed = true;
                        LNENDTime = hit.collider.GetComponent<ColNote>().getLnLength();
                    }
                    else
                    {
                        createhitef();
                        Destroy(hitup.collider.gameObject);
                    }
                }
                else
                {
                    error = errorDown;
                    notetiming = hitdown.collider.gameObject.GetComponent<ColNote>().getTime();
                    manager.AddError(erd);
                    if (hitdown.collider.gameObject.GetComponent<ColNote>().isLN())
                    {
                        isLNPRESSED = true;
                        hit = hitdown;
                        hit.collider.GetComponent<ColNote>().pressed = true;
                        LNENDTime = hit.collider.GetComponent<ColNote>().getLnLength();
                    }
                    else
                    {
                        createhitef();
                        Destroy(hitdown.collider.gameObject);
                    }
                }
                cacJudge(error);

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
        ef.GetComponent<scrLineEf>().delete();
        if (isLNPRESSED)
        {
            KeyInput(true, false);
            if (hit.collider != null)
                hit.collider.GetComponent<ColNote>().DestroyCollider();
        }
    }
    void cacJudge(float error)
    {

        if (error < 50f)
        {
            ScoreManager.combo++;
            getJudge(0);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error < 80f)
        {
            ScoreManager.combo++;
            getJudge(1);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error < 110f)
        {
            ScoreManager.combo++;
            getJudge(2);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else if (error < 135f)
        {
            ScoreManager.combo = 0;
            getJudge(3);
            if (isLNPRESSED) hit.collider.GetComponent<ColNote>().setPressed();
        }
        else
        {
            ScoreManager.combo = 0;
            getJudge(4);
        }
    }
    void cacLNJudge(float error)
    {

        if (error < 50f)
        {
            ScoreManager.combo++;
            getJudge(0);
        }
        else if (error < 100f)
        {
            ScoreManager.combo++;
            getJudge(1);
        }
        else if (error < 170f)
        {
            ScoreManager.combo++;
            getJudge(2);
        }
        else if (error < 230f)
        {
            ScoreManager.combo = 0;
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
        judgeobj.GetComponent<scrJudge>().setInfo(idx);
    }
}
