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
        transform.localScale = new Vector2(transform.localScale.x * Manager.ColumnWidth, transform.localScale.y);
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
        float xx = idx - 3f;
        if (Manager.keycount != 7)
        {
            switch (idx)
            {
                case 1:
                    xx = -1.5f;
                    break;
                case 2:
                    xx = -0.5f;
                    break;
                case 4:
                    xx = 0.5f;
                    break;
                case 5:
                    xx = 1.5f;
                    break;
            }
        }

        transform.position = new Vector2(xx * Manager.ColumnWidth, 1.28f  + Manager.stageYPOS);
    }
    public void setActive()
    {
        des = false; alph = 0.7f;

    }
}
