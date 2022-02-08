using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Judges;
using System;

/// <summary>
/// NEW INPUTSYSTEM FOR PC
/// </summary>

public class NewInputSystem : MonoBehaviour
{
    NotePlayer player;

    public bool[] isLNPRESSED = new bool[7]; //press 상태 column

    public int[] PressedColumn = new int[7]; //press 상태 column의 noteidx

    public GameObject[] Keys;

    List<int> Notes = new List<int>();



    ScoreManager manager;

    int curIndex;

    MusicHandler musicPlayer;

    [SerializeField]
    GameObject judgeobj;


    public static Action<int> _notehandle;


    void Awake()
    {
        player = GetComponent<NotePlayer>();
        musicPlayer = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        manager = GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player.NewInputSys)
        {
            if (!Manager.AutoPlay && NotePlayer.isLoaded)
                ManageHitables();

            //Notes = player.GetHitableNotes();
            if (Manager.keycount == 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (Input.GetKeyDown(KeySetting.keys[(KeyAction)i]))
                        KeyInput(i);
                    if (Input.GetKeyUp(KeySetting.keys[(KeyAction)i]))
                        KeyRelease(i);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    int n = -1;
                    if (i == 1) n = 0;
                    if (i == 2) n = 1;
                    if (i == 4) n = 2;
                    if (i == 5) n = 3;
                    if (n == -1) continue;
                    if (Input.GetKeyDown(KeySetting.keys[(KeyAction)i]))
                        KeyInput(n);
                    if (Input.GetKeyUp(KeySetting.keys[(KeyAction)i]))
                        KeyRelease(n);
                }
            }
            AutoKeyRelease();
        }
    }

    void ManageHitables()
    {
        if (player.NewInputSys && !Manager.AutoPlay)
        {
            if (curIndex < player.NoteList.Length)
            {
                if (player.NoteList[curIndex].TIME < NotePlayer.Playback + 3500 && !player.NoteList[curIndex].isCreated)
                {
                    Notes.Add(curIndex);
                    player.NoteList[curIndex].isCreated = true;
                    curIndex++;
                }
            }
            for (int i = 0; i < Notes.Count; i++)
            {
                float t = player.NoteList[Notes[i]].TIME;
                bool ln;
                if (player.NoteList[Notes[i]].ISLN)
                    ln = true;
                else
                    ln = false;

                if (t < NotePlayer.Playback - 180f)
                {
                    player.NoteList[Notes[i]].isDestroyed = true;

                    if (ln)
                    {
                        if (!player.NoteList[Notes[i]].isPressed)
                        {
                            manager.SetJudge(1);
                        }
                    }
                    else
                    {
                        manager.SetJudge(2);
                    }
                    Notes.RemoveAt(i);
                }
            }
        }
    }

    void KeyInput(int idx) //idx = 키보드 인덱스
    {
        int result = -1; //노트리스트 인덱스 결과
        float mTime = 99999f; //ABS적용 오차
        float rawError = 0; //원본오차
        int toRemove = -1;
        for (int i = 0; i < Notes.Count; i++)
        {
            var pn = player.NoteList[Notes[i]];
            float _rawError = pn.TIME - NotePlayer.Playback;
            if (Mathf.Abs(_rawError) < mTime && idx == pn.COLUMN)
            {
                mTime = Mathf.Abs(_rawError);
                result = Notes[i];
                toRemove = i;
                rawError = _rawError;
            }
        }
        if (result == -1)
            return;

        int ksindex = player.NoteList[result].KeySoundINDEX;
        if (ksindex != -1) //키사운드 플레이
            musicPlayer.PlaySample(ksindex);

        if (mTime < 174.4f)
        {

            //판정
            if (mTime < Timings.j300k) rawError = 0;
            manager.AddError(-rawError);
            cacJudge(idx, toRemove, result, mTime);
            if (!player.NoteList[result].ISLN)
            {
                CreateHitEffect(idx);
            }
            //delegate 입력전달 (노트제거)
            _notehandle(result);

        }
    }
    void KeyRelease(int idx)
    {
        if (isLNPRESSED[idx])
        {
            isLNPRESSED[idx] = false;
            var pn = player.NoteList[PressedColumn[idx]];
            float rawError = pn.LNLENGTH - NotePlayer.Playback;
            float error = Mathf.Abs(rawError);
            if (error <= LNTimings.j100)
            {
                if (error > LNTimings.j300k) rawError = 0;
                manager.AddError(-rawError);
            }
            cacLNJudge(error);
            CreateLNEffect(idx, false);
            //롱노트 놓을시 그래픽 변경
            _notehandle(PressedColumn[idx]);
        }
    }
    void AutoKeyRelease() //롱노트 계속 누르고 있을시 키 릴리즈 자동발생
    {
        for (int i = 0; i < isLNPRESSED.Length; i++)
        {
            if (isLNPRESSED[i])
            {
                if (player.NoteList[PressedColumn[i]].isPressed)
                {
                    if (player.NoteList[PressedColumn[i]].LNLENGTH < NotePlayer.Playback - 255f)
                    {
                        KeyRelease(i);
                    }
                }
            }
        }
    }
    void cacJudge(int idx, int toRemove, int result, float error)
    {
        NotePlayer.Notes n = player.NoteList[result];
        player.NoteList[result].isPressed = true;
        if (error <= Timings.j300k)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(0, toRemove);
            if (n.ISLN)
            {
                isLNPRESSED[idx] = true;
                CreateLNEffect(idx, true);
                PressedColumn[idx] = result;
            }
        }
        else if (error <= Timings.j300)
        {

            manager.score[manager.currentPlayer].COMBO++;
            getJudge(1, toRemove);
            if (n.ISLN)
            {
                isLNPRESSED[idx] = true;
                CreateLNEffect(idx, true);
                PressedColumn[idx] = result;
            }
        }

        else if (error <= Timings.j200)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(2, toRemove);
            if (n.ISLN)
            {
                isLNPRESSED[idx] = true;
                CreateLNEffect(idx, true);
                PressedColumn[idx] = result;
            }
        }
        else if (error <= Timings.j100)
        {
            manager.score[manager.currentPlayer].COMBO++;
            if (n.ISLN)
                getJudge(3, toRemove);
            else
                getJudge(6, toRemove);
            if (n.ISLN)
            {
                isLNPRESSED[idx] = true;
                CreateLNEffect(idx, true);
                PressedColumn[idx] = result;
            }
        }
        else
        {
            manager.score[manager.currentPlayer].COMBO = 0;
            if (n.ISLN)
                getJudge(5, toRemove);
            else
                getJudge(7, toRemove);
        }
    }
    void cacLNJudge(float error)
    {
        if (error <= LNTimings.j300k)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(0 , -1);
        }
        else if (error <= LNTimings.j300)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(1, -1);
        }
        else if (error <= LNTimings.j200)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(2, -1);
        }
        else if (error <= LNTimings.j100)
        {
            manager.score[manager.currentPlayer].COMBO++;
            getJudge(3, -1);
        }
        else
        {
            manager.score[manager.currentPlayer].COMBO = 0;
            getJudge(4, -1);
        }
        
    }
    void getJudge(int idx, int toRemove)
    {
        if (toRemove != -1)
            Notes.RemoveAt(toRemove);
        judgeobj.SetActive(true);
        judgeobj.GetComponent<Judgement>().setInfo(idx);
    }
    void CreateHitEffect(int idx)
    {
        if (Manager.keycount == 7)
            Keys[idx].GetComponent<KeyInputManager>().createhitef();
        else
        {
            int n = ConvertKeyTo4K(idx);
            Keys[n].GetComponent<KeyInputManager>().createhitef();
        }
    }
    void CreateLNEffect(int idx, bool on)
    {
        int nn = ConvertKeyTo4K(idx);
        Keys[nn].GetComponent<KeyInputManager>().ToggleLNEffect(on);
    }
    int ConvertKeyTo4K(int idx)
    {
        if (Manager.keycount == 7) return idx;
        int n = 0;
        if (idx == 0) n = 1;
        if (idx == 1) n = 2;
        if (idx == 2) n = 4;
        if (idx == 3) n = 5;
        return n;
    }
}
