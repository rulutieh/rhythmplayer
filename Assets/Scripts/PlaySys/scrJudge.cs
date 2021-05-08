using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrJudge : MonoBehaviour
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
        Vector2 target = new Vector2(transform.position.x, -0.71f);
        transform.position = Vector3.MoveTowards(transform.position, target, 4f * Time.deltaTime);
        Color tmp = rend.color;
        tmp.a = alph;
        alph -= 1f * Time.deltaTime;
        rend.color = tmp;

        if (alph <= 0.1f) gameObject.SetActive(false);
    }
    public void setInfo(int j)
    {
        transform.position = new Vector2(0, -0.86f);
        alph = 1;
        FileReader.TOTAL++;
        switch (j)
        {
            case 0:
                //kool
                rend.sprite = spr[0];
                FileReader.COOL++;
                FileReader.HP += scrSetting.hprecover;

                break;
            case 1:
                rend.sprite = spr[1];
                FileReader.GREAT++;
                FileReader.HP += scrSetting.hprecover2; 

                break;
            case 2:
                rend.sprite = spr[2];
                FileReader.GOOD++;

                break;
            case 3:
                rend.sprite = spr[3];
                FileReader.BAD++;

                FileReader.HP -= scrSetting.baddamage;
                break;
            case 4:
                //miss
                rend.sprite = spr[4];
                FileReader.MISS++;
                FileReader.HP -= scrSetting.missdamage;
                break;
            case 5:
                //miss ln
                rend.sprite = spr[4];
                FileReader.MISS += 2;
                FileReader.TOTAL++;
                FileReader.HP -= scrSetting.missdamage * 2f;
                break;
        }
        if (FileReader.HP > 1f) FileReader.HP = 1f;
    }
    
}
