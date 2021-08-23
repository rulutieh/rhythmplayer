using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Health;

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
                rend.sprite = spr[5];
                ScoreManager.KOOL++;
                ScoreManager.HP += Recovery.max;
                break;
            case 1:
                //cool
                rend.sprite = spr[0];
                ScoreManager.COOL++;
                ScoreManager.HP += Recovery.max;
                break;
            case 2:
                rend.sprite = spr[1];
                ScoreManager.GOOD++;
                ScoreManager.HP += Recovery.good;
                break;
            case 3:
                rend.sprite = spr[3];
                ScoreManager.BAD++;
                ScoreManager.HP -= Damage.bad;
                break;
            case 4:
                //miss
                rend.sprite = spr[4];
                ScoreManager.MISS++;
                ScoreManager.HP -= Damage.miss;
                break;
            case 5:
                //miss
                rend.sprite = spr[4];
                ScoreManager.MISS += 2;
                ScoreManager.TOTAL++;
                ScoreManager.HP -= Damage.miss * 2;
                break;
        }
        if (ScoreManager.HP > 1f) ScoreManager.HP = 1f;
    }
    
}
