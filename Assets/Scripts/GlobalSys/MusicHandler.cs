using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

public class MusicHandler : MonoBehaviour
{
    FMOD.ChannelGroup channelGroup = new FMOD.ChannelGroup();
    FMOD.Channel[] channel = new FMOD.Channel[8];
    FMOD.Sound snd;
    FMOD.Sound[] sfx = new FMOD.Sound[20];
    FMOD.RESULT isLoadDone;
    bool isplaying;
    uint length;
    FMOD.Studio.EVENT_CALLBACK dialogueCallback;


    // Start is called before the first frame update
    void Start()
    {
        //SND

        FMODUnity.RuntimeManager.CoreSystem.createChannelGroup("cg", out channelGroup); //채널그룹생성
        for (int i = 0; i < 8; i++) //할당
        {
            channel[i] = new FMOD.Channel();
            channel[i].setChannelGroup(channelGroup);
        }
        FMODUnity.RuntimeManager.CoreSystem.setDSPBufferSize(256, 4); //로우레이턴시 DSP

        //SFX

        StreamReader rdr = new StreamReader(Path.Combine(Application.streamingAssetsPath, "SFXs", "sfx.ini"));
        string line;
        int idx = 0;
        while ((line = rdr.ReadLine()) != null)
        {
            string root = Path.Combine(Application.streamingAssetsPath, "SFXs", line);
            if (File.Exists(root))
                LoadSfx(idx++, root);
        }
        rdr.Close();
    }
    public void LoadSfx(int idx, string path)
    {
        sfx[idx] = new FMOD.Sound();
        FMODUnity.RuntimeManager.CoreSystem.createSound(path, FMOD.MODE.CREATESAMPLE, out sfx[idx]);
    }
    public void LoadSound(string fpath)
    {
        snd = new FMOD.Sound();
        isLoadDone = FMODUnity.RuntimeManager.CoreSystem.createSound(fpath, FMOD.MODE.CREATESAMPLE | FMOD.MODE.ACCURATETIME, out snd);
        //fmodresult = 경로, 샘플생성 | accurate모드 플래그, 주소

    }
    public void PlayMP3()
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(snd, channelGroup, false, out channel[0]); //재생
        snd.getLength(out length, FMOD.TIMEUNIT.MS); // 곡 길이
        channelGroup.setVolume(scrSetting.Volume); //볼륨

    }
    public void PlaySFX(int idx)
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(sfx[idx], channelGroup, false, out channel[7]);
        channelGroup.setVolume(scrSetting.Volume);
    }
    public void StopMP3()
    {
        channelGroup.isPlaying(out isplaying);
        if (isplaying)
        {
            channelGroup.stop();
        }
        //정지
    }
    public void ReleaseMP3()
    {
        snd.release();
        //사운드 메모리 해제
    }
    public uint GetLength()
    {
        return length;
        //곡 길이 측정
    }
    public FMOD.OPENSTATE isLoaded()
    {
        FMOD.OPENSTATE state;
        bool disk, starving;
        uint percent;
        snd.getOpenState(out state, out percent, out starving, out disk);
        return state;
        //코루틴 로딩완료 체크
    }
    void OnApplicationQuit()
    {
        snd.release();
        for (int i = 0; i < sfx.Length; i++)
        {
            sfx[i].release();
        }
#if !UNITY_EDITOR
        FMODUnity.RuntimeManager.CoreSystem.release();
#endif
        //시스템 메모리 해제
    }
}
