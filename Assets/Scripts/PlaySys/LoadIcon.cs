using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadIcon : MonoBehaviour
{
    Image[] images;
    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<Image>();
    }
    public void Fade()
    {

        foreach(Image img in images)
        {
            img.DOColor(new Color(1, 1, 1, 0), 1f);
        }
    }
}
