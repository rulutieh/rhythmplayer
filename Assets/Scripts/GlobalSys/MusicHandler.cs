using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

public class MusicHandler : MonoBehaviour
{
    FMOD.ChannelGroup channelGroup = new FMOD.ChannelGroup();
    FMOD.Channel[] channel = new FMOD.Channel[1000];
    FMOD.Sound snd;
    FMOD.Sound[] sfx = new FMOD.Sound[20];
    FMOD.RESULT isLoadDone = FMOD.RESULT.ERR_FILE_NOTFOUND;
    bool isplaying;
    uint length;
    FMOD.Studio.EVENT_CALLBACK dialogueCallback;

    int samplechannelidx = 26;
    List<FMOD.Sound> KeySoundList = new List<FMOD.Sound>();

    void Start()
    {
        //SND

        
        var e = FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        Debug.Log(e);
        for (int i = 0; i < 1000; i++) //할당
        {
            channel[i] = new FMOD.Channel();
            channel[i].setChannelGroup(channelGroup);
        }

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
    }
    public void LoadKeySound(string fpath)
    {
        var ks = new FMOD.Sound();
        FMODUnity.RuntimeManager.CoreSystem.createSound(fpath, FMOD.MODE.CREATESAMPLE, out ks);
        KeySoundList.Add(ks);

    }
    public void PlayMP3()
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(snd, channelGroup, false, out channel[0]); //재생
        snd.getLength(out length, FMOD.TIMEUNIT.MS); // 곡 길이
        channelGroup.setVolume(GlobalSettings.Volume); //볼륨
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out uint a, out int b);
        Debug.Log(a);
    }
    public void PlaySFX(int idx)
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(sfx[idx], channelGroup, false, out channel[1]);
        channelGroup.setVolume(GlobalSettings.Volume);
    }
    public void PlaySample(int idx)
    {
        bool isplaying = true;
        samplechannelidx = 2;
        while (!isplaying)
        {
            channel[samplechannelidx].isPlaying(out isplaying);
            samplechannelidx++;
            if (samplechannelidx == 1000)
            {
                samplechannelidx = (int)Random.Range(2f, 1000f);
                channel[samplechannelidx].stop();
                isplaying = false;
            }
        }
        FMODUnity.RuntimeManager.CoreSystem.playSound(KeySoundList[idx], channelGroup, false, out channel[samplechannelidx]); //재생
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
    public void ReleaseKeysound()
    {
        for (int i = 0; i < KeySoundList.Count; i++)
        {
            KeySoundList[i].release();
        }

        KeySoundList = new List<FMOD.Sound>();
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
        ReleaseKeysound();
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
