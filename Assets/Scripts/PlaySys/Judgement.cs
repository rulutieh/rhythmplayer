using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Health;

public class Judgement : MonoBehaviour
{
    float alph = 1f;
    public int c;
    SpriteRenderer rend;
    [SerializeField]
    GameObject score;
    ScoreManager manager;
    public Sprite[] spr;
    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        manager = score.GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 target = new Vector2(transform.position.x, -1.2f + Manager.stageYPOS);
        transform.position = Vector3.MoveTowards(transform.position, target, 4f * Time.deltaTime);
        Color tmp = rend.color;
        tmp.a = alph;
        alph -= 1f * Time.deltaTime;
        rend.color = tmp;

        if (alph <= 0.1f) gameObject.SetActive(false);

    }
    public void setInfo(int j)
    {
        transform.position = new Vector2(0, -1f + Manager.stageYPOS);
        alph = 1;
        manager.score[manager.currentPlayer].TOTAL++;
        switch (j)
        {
            case 0:
                //kool (롱놋)
                rend.sprite = spr[5];
                manager.score[manager.currentPlayer].KOOL++;
                ScoreManager.HP += Recovery.max;
                break;
            case 1:
                //cool (롱놋)
                rend.sprite = spr[0];
                manager.score[manager.currentPlayer].COOL++;
                ScoreManager.HP += Recovery.max;
                break;
            case 2:
                rend.sprite = spr[1];
                manager.score[manager.currentPlayer].GOOD++;
                ScoreManager.HP += Recovery.good;
                break;
            case 3:
                rend.sprite = spr[3];
                manager.score[manager.currentPlayer].BAD++;
                ScoreManager.HP -= Damage.bad;
                break;
            case 4:
                //miss
                rend.sprite = spr[4];
                manager.score[manager.currentPlayer].MISS++;
                ScoreManager.HP -= Damage.miss;
                break;
            case 5:
                //miss2
                rend.sprite = spr[4];
                manager.score[manager.currentPlayer].MISS += 2;
                manager.score[manager.currentPlayer].TOTAL++;
                ScoreManager.HP -= Damage.miss * 2;
                break;

            case 6:
                //bad  (단놋)
                rend.sprite = spr[3];
                manager.score[manager.currentPlayer].BAD++;
                ScoreManager.HP -= Damage.bad * 1.5f;
                break;
            case 7:
                //miss(단놋)
                rend.sprite = spr[4];
                manager.score[manager.currentPlayer].MISS++;
                ScoreManager.HP -= Damage.miss * 1.5f;
                break;
        }
        if (ScoreManager.HP > 1f) ScoreManager.HP = 1f;

        manager.CacScore();
    }
    
}
