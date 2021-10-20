using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;

public class NullFileError : MonoBehaviour
{
    public Image gutter;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;
    public Transform text;
    // Start is called before the first frame update
    //void OnEnable()
    //{
    //    gutterStartSize = new Vector2(0, gutter.rectTransform.sizeDelta.y);
    //    gutterEndSize = gutter.rectTransform.sizeDelta;
    //    gutter.rectTransform.sizeDelta = new Vector2(0, 0);
    //    Tween.Size(gutter.rectTransform, gutterStartSize, gutterEndSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => showChilds());
    //}
    //void showChilds()
    //{
    //    text.gameObject.SetActive(true);
    //    StartCoroutine(Close());
    //}
    //IEnumerator Close()
    //{
    //    yield return new WaitForSeconds(3f);
    //    text.gameObject.SetActive(false);
    //    Tween.Size(gutter.rectTransform,  gutterEndSize, gutterStartSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None);
    //}

}
