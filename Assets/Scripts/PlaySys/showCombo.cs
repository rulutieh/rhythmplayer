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
    float scale = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        tmpCombo = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {

        if (FileReader.combo > 2)
        {
            if (alpha < 1f) alpha += Time.deltaTime;
        }
        else
        {
            if (alpha > 0) alpha -= Time.deltaTime * 30f;
        }
        if (oldcombo != FileReader.combo)
        {
            tmpCombo.text = FileReader.combo.ToString();
            oldcombo = FileReader.combo;
            scale = 1.35f;           
        }
        tmpCombo.color = new Color(255, 255, 255, alpha);
        tmp.color = new Color(255, 255, 255, alpha);
        transform.localScale = new Vector2(scale, scale);
        if (scale > 1.1f) scale -= 5f * Time.deltaTime;
    }
}
