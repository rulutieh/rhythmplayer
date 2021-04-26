using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playField : MonoBehaviour
{
    RectTransform rect;
    float yy = -3.1778f;
    public GameObject judgeline, scoreran;
    // Start is called before the first frame update
    void Start()
    {
        rect = scoreran.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) scrSetting.stageXPOS += 0.2f;
            if (Input.GetKeyDown(KeyCode.RightArrow)) scrSetting.stageXPOS -= 0.2f;
            if (Input.GetKeyDown(KeyCode.UpArrow)) scrSetting.stageYPOS += 0.1f;
            if (Input.GetKeyDown(KeyCode.DownArrow)) scrSetting.stageYPOS -= 0.1f;

            if (scrSetting.stageXPOS > 5f) scrSetting.stageXPOS = 5f;
            if (scrSetting.stageXPOS < -5f) scrSetting.stageXPOS = -5f;
            if (scrSetting.stageYPOS > 1f) scrSetting.stageYPOS = 1f;
            if (scrSetting.stageXPOS < -0.2f) scrSetting.stageYPOS = -0.2f;
        }
        if (scrSetting.stageXPOS > 0)
        {
            rect.anchoredPosition = new Vector2(-258, -126f);
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
        }
        if (scrSetting.stageXPOS < 0)
        {
            rect.anchoredPosition = new Vector2(260f, -126f);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
        }
        transform.position = new Vector3(scrSetting.stageXPOS, transform.position.y, transform.position.z);
        judgeline.transform.position = new Vector2(0, yy + scrSetting.stageYPOS);
    }
}
