using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffects : MonoBehaviour
{
    float alph = 0.7f;
    int col;
    bool des = false;
    SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(transform.localScale.x * GlobalSettings.ColWidth, transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {
        Color tmp = rend.color;
        tmp.a = alph;
        rend.color = tmp;

        if (des)
        {
            alph -= 5f * Time.deltaTime;
            if (alph <= 0.1f) Destroy(gameObject);
            transform.Translate(Vector2.down * Time.deltaTime * 3.2f);
        }
    }

    public void delete()
    {
        des = true;
    }
    public void setCol(int idx)
    {
        transform.position = new Vector2((idx - 3f) * GlobalSettings.ColWidth, 1.28f + GlobalSettings.stageYPOS);
    }
    public void setActive()
    {
        des = false; alph = 0.7f;

    }
}
