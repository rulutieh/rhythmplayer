using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

public class scrUI : MonoBehaviour
{
    public Image gutter;
    public int num = 1;
    float _delay;
    Transform[] allChildren;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;

    private void Awake()
    {
        SettingPanel.isUIanimationFinished = false;
        gutterStartSize = new Vector2(gutter.rectTransform.sizeDelta.x, 0);
        gutterEndSize = gutter.rectTransform.sizeDelta;
        gutter.rectTransform.sizeDelta = new Vector2(0, 0);
        hideChilds();
    }
    public void Active()
    {
        SettingPanel.isUIanimationFinished = false;
        transform.GetChild(0).gameObject.SetActive(true);
        Tween.Size(gutter.rectTransform, gutterStartSize, gutterEndSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => showChilds());
        
    }
    public void inActive()
    {
        hideChilds();
        Tween.Size(gutter.rectTransform, gutterEndSize, gutterStartSize, .5f, 0, Tween.EaseInOutStrong);

    }
    void showChilds()
    {
        SettingPanel.isUIanimationFinished = true;
        foreach (Transform child in transform)
        {
            if (child.name != transform.name)
                child.gameObject.SetActive(true);
        }
    }
    void hideChilds()
    {
        foreach (Transform child in transform)
        {
            if (child.name != transform.name && child.name != transform.GetChild(0).name)
                child.gameObject.SetActive(false);
        }
    }
}
