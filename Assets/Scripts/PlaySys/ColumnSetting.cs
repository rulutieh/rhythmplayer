using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColumnSetting : MonoBehaviour
{
    public GameObject blur, fade, line, buttom;
    public GameObject left, right, hpleft, hpright;
    public bool isJudgeline;
    // Start is called before the first frame update
    void Start()
    {
        float colw = Manager.ColWidth;
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        if (!isJudgeline)
        {
            left.transform.localPosition = new Vector2(-3.5f * colw, 0.2f);
            right.transform.localPosition = new Vector2(3.5f * colw, 0.2f);
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(1, 1, 1, 0);
            sprites[i].DOColor(new Color(1, 1, 1, 1), 2f);
        }
        //colwidth 재설정
        if (blur)
        {
            blur.transform.localScale = fade.transform.localScale = new Vector2(2.83f * colw, 5.14f);
            line.transform.localScale = new Vector2(4.03f * colw, 1.5f);
            buttom.transform.localScale = new Vector2(1.35f * colw, 1.46f);
        }
    }

    // Update is called once per frame
    public void onResult()
    {
        if (blur) blur.SetActive(false);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].DOColor(new Color(1, 1, 1, 0), 2f);
        }
    }
    void Update()
    {

    }
}
