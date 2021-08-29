using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playField : MonoBehaviour
{
    float yy = -3.1778f;
    public GameObject judgeline, scoreran;
    public Image fade;
    // Start is called before the first frame update
    // Update is called once per frame
    private void Awake()
    {
        SetFade();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) GlobalSettings.stageXPOS += 0.2f;
            if (Input.GetKeyDown(KeyCode.RightArrow)) GlobalSettings.stageXPOS -= 0.2f;
            if (Input.GetKeyDown(KeyCode.UpArrow)) GlobalSettings.stageYPOS += 0.1f;
            if (Input.GetKeyDown(KeyCode.DownArrow)) GlobalSettings.stageYPOS -= 0.1f;

            if (GlobalSettings.stageXPOS > 5f) GlobalSettings.stageXPOS = 5f;
            if (GlobalSettings.stageXPOS < -5f) GlobalSettings.stageXPOS = -5f;
            if (GlobalSettings.stageYPOS > 1f) GlobalSettings.stageYPOS = 1f;
            if (GlobalSettings.stageXPOS < -0.2f) GlobalSettings.stageYPOS = -0.2f;
        }

        transform.position = new Vector3(GlobalSettings.stageXPOS, transform.position.y, transform.position.z);
        judgeline.transform.position = new Vector2(0, yy + GlobalSettings.stageYPOS);

        if (Input.GetKeyDown(KeyCode.PageDown) && GlobalSettings.Transparency > 0)
        { GlobalSettings.Transparency -= 0.2f; SetFade(); }
        if (Input.GetKeyDown(KeyCode.PageUp) && GlobalSettings.Transparency < 1)
        { GlobalSettings.Transparency += 0.2f; SetFade(); }
    }
    void SetFade()
    {
        fade.color = new Color(0, 0, 0, 1f - GlobalSettings.Transparency);
    }
}
