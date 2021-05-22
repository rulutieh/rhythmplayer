using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgement : MonoBehaviour
{
    float alph = 1f;
    public int c;
    SpriteRenderer rend;
    public Sprite[] spr;
    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 target = new Vector2(transform.position.x, -1.2f + GlobalSettings.stageYPOS);
        transform.position = Vector3.MoveTowards(transform.position, target, 4f * Time.deltaTime);
        Color tmp = rend.color;
        tmp.a = alph;
        alph -= 1f * Time.deltaTime;
        rend.color = tmp;

        if (alph <= 0.1f) gameObject.SetActive(false);

    }
    public void setInfo(int j)
    {
        transform.position = new Vector2(0, -1f + GlobalSettings.stageYPOS);
        alph = 1;
        ScoreManager.TOTAL++;
        switch (j)
        {
            case 0:
                //kool
                rend.sprite = spr[0];
                ScoreManager.COOL++;
                ScoreManager.HP += GlobalSettings.hprecover;

                break;
            case 1:
                rend.sprite = spr[1];
                ScoreManager.GREAT++;
                ScoreManager.HP += GlobalSettings.hprecover2; 

                break;
            case 2:
                rend.sprite = spr[2];
                ScoreManager.GOOD++;

                break;
            case 3:
                rend.sprite = spr[3];
                ScoreManager.BAD++;

                ScoreManager.HP -= GlobalSettings.baddamage;
                break;
            case 4:
                //miss
                rend.sprite = spr[4];
                ScoreManager.MISS++;
                ScoreManager.HP -= GlobalSettings.missdamage;
                break;
            case 5:
                //miss ln
                rend.sprite = spr[4];
                ScoreManager.MISS += 2;
                ScoreManager.TOTAL++;
                ScoreManager.HP -= GlobalSettings.missdamage * 2f;
                break;
        }
        if (ScoreManager.HP > 1f) ScoreManager.HP = 1f;
    }
    
}
