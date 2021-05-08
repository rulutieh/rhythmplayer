using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;


public class playerinit : MonoBehaviour
{
    public GameObject[] elements;
    Transform[] allChildren;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;
    Image gutter;
    // Start is called before the first frame update
    void Start()
    {
        gutter = GetComponent<Image>();
        gutterStartSize = new Vector2(gutter.rectTransform.sizeDelta.x, 0);
        gutterEndSize = gutter.rectTransform.sizeDelta;
        gutter.rectTransform.sizeDelta = new Vector2(0, 0);

        Tween.Size(gutter.rectTransform, gutterStartSize, gutterEndSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => showChilds());
    }
    void showChilds()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].SetActive(true);
    }
    public void hideChilds()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].SetActive(false);
        Tween.Size(gutter.rectTransform, gutterEndSize, gutterStartSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => Destroy(gameObject));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
