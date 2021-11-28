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
    ScoreManager manager;
    // Start is called before the first frame update
    void Start()
    {
        tmpCombo = gameObject.GetComponent<TextMeshPro>();
        col = Manager.ColWidth;
        var g = GameObject.FindWithTag("NoteSys");
        manager = g.GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (manager.score[manager.currentPlayer].COMBO > 2)
        {
            if (alpha < 1f) alpha += Time.deltaTime;
        }
        else
        {
            if (alpha > 0) alpha -= Time.deltaTime * 30f;
        }
        if (oldcombo != manager.score[manager.currentPlayer].COMBO)
        {
            tmpCombo.text = manager.score[manager.currentPlayer].COMBO.ToString();
            oldcombo = manager.score[manager.currentPlayer].COMBO;
            scale = 1.4f * col;           
        }
        tmpCombo.color = new Color(255, 255, 255, alpha);
        tmp.color = new Color(255, 255, 255, alpha);
        transform.localScale = new Vector2(scale, scale);
        if (scale > 1.15f * col) scale -= 5f * Time.deltaTime;
    }
}
