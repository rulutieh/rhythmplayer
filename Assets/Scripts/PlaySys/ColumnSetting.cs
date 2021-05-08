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
        float colw = scrSetting.ColWidth / 0.85f;
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        if (!isJudgeline)
        {
            left.transform.localPosition = new Vector2(-2.96f * colw, 0.2f);
            right.transform.localPosition = new Vector2(+2.96f * colw, 0.2f);
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(1, 1, 1, 0);
            sprites[i].DOColor(new Color(1, 1, 1, 1), 2f);
        }
        //colwidth 재설정
        if (blur)
        {
            blur.transform.localScale = fade.transform.localScale = new Vector2(2.41f * scrSetting.ColWidth / 0.85f, 5.14f);
            line.transform.localScale = new Vector2(3.43f * colw, 1.5f);
            buttom.transform.localScale = new Vector2(1.14f * colw, 1.46f);
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
