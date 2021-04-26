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
    // Start is called before the first frame update
    // Update is called once per frame
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        switch (idx)
            {
                case 0:
                    if (Input.GetKey(KeySetting.keys[KeyAction._0]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._0]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._0]))
                        KeyRelease();
                    break;
                case 1:
                    if (Input.GetKey(KeySetting.keys[KeyAction._1]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._1]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._1]))
                        KeyRelease();
                    break;
                case 2:
                    if (Input.GetKey(KeySetting.keys[KeyAction._2]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._2]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._2]))
                        KeyRelease();
                    break;
                case 3:
                    if (Input.GetKey(KeySetting.keys[KeyAction._3]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._3]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._3]))
                        KeyRelease();
                    break;
                case 4:
                    if (Input.GetKey(KeySetting.keys[KeyAction._4]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._4]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._4]))
                        KeyRelease();
                    break;
                case 5:
                    if (Input.GetKey(KeySetting.keys[KeyAction._5]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._5]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._5]))
                        KeyRelease();
                    break;
                case 6:
                    if (Input.GetKey(KeySetting.keys[KeyAction._6]))
                        rend.sprite = PRESS;
                    else
                        rend.sprite = DEF;
                    if (Input.GetKeyDown(KeySetting.keys[KeyAction._6]))
                        KeyInput(isLNPRESSED, true);
                    if (Input.GetKeyUp(KeySetting.keys[KeyAction._6]))
                        KeyRelease();
                    break;
            }

        if (isLNPRESSED) //롱놋 안 땠을 시 미스처리
        {
            float error = FileReader.Playback - LNENDTime;
            if (error > 270f)
            {
                FileReader.combo = 0;
                getJudge(4);
                isLNPRESSED = false;
            }
            lnhitef.gameObject.SetActive(true);
        } else lnhitef.gameObject.SetActive(false);

        transform.position = new Vector2((idx - 3) * scrSetting.ColWidth, transform.position.y);
        

    }
    void KeyInput(bool LN, bool effect)
    {
        if (effect)
        {
            ef = Instantiate(lineEf); //라인이펙트
            ef.GetComponent<scrLineEf>().setCol(idx);
        }

        isLNPRESSED = false;
        float errorUp;
        float errorDown;
        float error;
        float notetiming;
        if (LN)
        {
            error = Mathf.Abs(FileReader.Playback - LNENDTime);
            cacLNJudge(error);
        }
        else
        {
            RaycastHit2D hitup, hitdown;
            hitup = Physics2D.Raycast(transform.position, Vector2.up); //위로 레이
            hitdown = Physics2D.Raycast(transform.position, Vector2.down); //아래로 레이
            if (hitup.collider != null)
            { //위 체크
                errorUp = Mathf.Abs(FileReader.Playback - hitup.collider.GetComponent<scrNote>().getTime()); //현재시간에서 노트의 시간값 빼서 오차가져오기
            }
            else errorUp = 9999f;
            if (hitdown.collider != null)
            { //아래 체크
                errorDown = Mathf.Abs(FileReader.Playback - hitdown.collider.GetComponent<scrNote>().getTime());
            }
            else errorDown = 9999f;

            if (errorUp <= 178.4f || errorDown <= 178.4f) 
            {
                if (errorUp < errorDown)
                {
                    error = errorUp;
                    notetiming = hitup.collider.gameObject.GetComponent<scrNote>().getTime(); //가까운 노트의 목표타이밍값 가져오기

                    if (hitup.collider.gameObject.GetComponent<scrNote>().isLN())
                    {
                        isLNPRESSED = true;
                        hit = hitup;
                        hit.collider.GetComponent<scrNote>().pressed = true;
                        LNENDTime = hit.collider.GetComponent<scrNote>().getLnLength();
                    }
                    else
                    {
                        Instantiate(hitef, transform.position, Quaternion.identity);
                        Destroy(hitup.collider.gameObject);
                    }
                }
                else
                {
                    error = errorDown;
                    notetiming = hitdown.collider.gameObject.GetComponent<scrNote>().getTime();
                    if (hitdown.collider.gameObject.GetComponent<scrNote>().isLN())
                    {
                        isLNPRESSED = true;
                        hit = hitdown;
                        hit.collider.GetComponent<scrNote>().pressed = true;
                        LNENDTime = hit.collider.GetComponent<scrNote>().getLnLength();
                    }
                    else
                    {
                        Instantiate(hitef, transform.position, Quaternion.identity);
                        Destroy(hitdown.collider.gameObject);
                    }
                }
                cacJudge(error);
                
            }
        }
    }
    void KeyRelease()
    {
        ef.GetComponent<scrLineEf>().delete();
        if (isLNPRESSED)
        {
            KeyInput(true, false);
            if (hit.collider != null)
                hit.collider.GetComponent<scrNote>().DestroyCollider();
        }
    }
    void cacJudge(float error)
    {

        if (error < 50f)
        {
            FileReader.combo++;
            getJudge(0);
            if (isLNPRESSED) hit.collider.GetComponent<scrNote>().setPressed();
        }
        else if (error < 100f)
        {
            FileReader.combo++;
            getJudge(1);
            if (isLNPRESSED) hit.collider.GetComponent<scrNote>().setPressed();
        }
        else if (error < 130f)
        {
            FileReader.combo++;
            getJudge(2);
            if (isLNPRESSED) hit.collider.GetComponent<scrNote>().setPressed();
        }
        else if (error < 160f)
        {
            FileReader.combo = 0;
            getJudge(3);
            if (isLNPRESSED) hit.collider.GetComponent<scrNote>().setPressed();
        }
        else
        {
            FileReader.combo = 0;
            getJudge(4);
        }
    }
    void cacLNJudge(float error)
    {

        if (error < 50f)
        {
            FileReader.combo++;
            getJudge(0);
        }
        else if (error < 100f)
        {
            FileReader.combo++;
            getJudge(1);
        }
        else if (error < 170f)
        {
            FileReader.combo++;
            getJudge(2);
        }
        else if (error < 230f)
        {
            FileReader.combo = 0;
            getJudge(3);
        }
        else
        {
            FileReader.combo = 0;
            getJudge(4);
        }
    }
    void getJudge(int idx)
    {
        judgeobj.SetActive(true);
        judgeobj.GetComponent<scrJudge>().setInfo(idx);
    }
}
