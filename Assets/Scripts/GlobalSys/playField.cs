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
            if (Input.GetKeyDown(KeyCode.LeftArrow)) Manager.stageXPOS += 0.2f;
            if (Input.GetKeyDown(KeyCode.RightArrow)) Manager.stageXPOS -= 0.2f;
            if (Input.GetKeyDown(KeyCode.UpArrow)) Manager.stageYPOS += 0.1f;
            if (Input.GetKeyDown(KeyCode.DownArrow)) Manager.stageYPOS -= 0.1f;

            if (Manager.stageXPOS > 5f) Manager.stageXPOS = 5f;
            if (Manager.stageXPOS < -5f) Manager.stageXPOS = -5f;
            if (Manager.stageYPOS > 1f) Manager.stageYPOS = 1f;
            if (Manager.stageXPOS < -0.2f) Manager.stageYPOS = -0.2f;
        }

        transform.position = new Vector3(Manager.stageXPOS, transform.position.y, transform.position.z);
        judgeline.transform.position = new Vector2(0, yy + Manager.stageYPOS);

        if (Input.GetKeyDown(KeyCode.PageDown) && Manager.Transparency > 0)
        { Manager.Transparency -= 0.2f; SetFade(); }
        if (Input.GetKeyDown(KeyCode.PageUp) && Manager.Transparency < 1)
        { Manager.Transparency += 0.2f; SetFade(); }
    }
    void SetFade()
    {
        fade.color = new Color(0, 0, 0, 1f - Manager.Transparency);
    }
}
