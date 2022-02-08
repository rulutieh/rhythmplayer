using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float NoteTiming;
    public NotePlayer rdr;
    public int Index;
    public bool ISLN;
    public bool isPressed;
    public bool LnReleased;

    public void NoteInit()
    {
        var sys = GameObject.FindWithTag("NoteSys");
        rdr = sys.GetComponent<NotePlayer>();
        
    }

    public void NoteAppear()
    {
        NewInputSystem._notehandle += Pressed;
        isPressed = false;
        LnReleased = false;
    }
    public void NoteDisappear()
    {
        NewInputSystem._notehandle -= Pressed;
    }

    public void Pressed(int idx)
    {
        if (idx == Index)
        {
            if (!isPressed) //노트 렌더 제거
                isPressed = true;
            else if (ISLN)  //롱놋 놓칠시 그래픽 변경
                LnReleased = true;
        }

    }

    private void LateUpdate()
    {
        float pos = (float)(NotePlayer.judgeoffset + (NoteTiming - NotePlayer.PlaybackChanged) * NotePlayer.multiply);
        transform.position = new Vector2(transform.position.x, pos);
    }
}
