using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class showCombo : MonoBehaviour
{
    float alpha;
    int oldcombo;
    TextMeshPro tmpCombo;
    public TextMeshPro tmp;
    float scale = 1.1f, col;
    // Start is called before the first frame update
    void Start()
    {
        tmpCombo = gameObject.GetComponent<TextMeshPro>();
        col = scrSetting.ColWidth / 0.85f;
    }

    // Update is called once per frame
    void Update()
    {

        if (ScoreManager.combo > 2)
        {
            if (alpha < 1f) alpha += Time.deltaTime;
        }
        else
        {
            if (alpha > 0) alpha -= Time.deltaTime * 30f;
        }
        if (oldcombo != ScoreManager.combo)
        {
            tmpCombo.text = ScoreManager.combo.ToString();
            oldcombo = ScoreManager.combo;
            scale = 1.35f * col;           
        }
        tmpCombo.color = new Color(255, 255, 255, alpha);
        tmp.color = new Color(255, 255, 255, alpha);
        transform.localScale = new Vector2(scale, scale);
        if (scale > 1.1f * col) scale -= 5f * Time.deltaTime;
    }
}
