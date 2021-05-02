using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class playerAlpha : MonoBehaviour
{
    public GameObject blur;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(1, 1, 1, 0);
            sprites[i].DOColor(new Color(1, 1, 1, 1), 2f);
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
