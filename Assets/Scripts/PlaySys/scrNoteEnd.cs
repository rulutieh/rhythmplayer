using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrNoteEnd : MonoBehaviour
{
    public bool isPressed;
    float TIME, _TIME;
    public Sprite dk, sp;
    SpriteRenderer rend;

    private void LateUpdate()
    {
        //transform.Translate(Vector2.down * scrReadFile.defaultvec * scrReadFile.multiply * scrSelect.scrollSpeed * Time.smoothDeltaTime);
        transform.position = new Vector2(transform.position.x, (float)(FileReader.judgeoffset + (_TIME - FileReader.PlaybackChanged) * FileReader.multiply));
    }

    public void setInfo(int c, float t, GameObject obj, float nt) //콜룸, 타임, 시작노트
    {
        rend = GetComponent<SpriteRenderer>();
        if (c == 1 || c == 5) { rend.sprite = dk;} if (c == 3) { rend.sprite = sp; }
        TIME = t; _TIME = nt;
        if (obj)
            transform.position = new Vector2(obj.transform.position.x, transform.position.y);
        else Destroy(gameObject);
        ///////////////////////////////////////


    }
    public float getTime()
    {
        return TIME;
    }
}
