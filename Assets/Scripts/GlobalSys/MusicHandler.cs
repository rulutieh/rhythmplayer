using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class MusicHandler : MonoBehaviour
{
    FMOD.ChannelGroup channelGroup = new FMOD.ChannelGroup();
    FMOD.Channel[] channel = new FMOD.Channel[8];
    FMOD.Sound snd;
    bool isplaying;

    FMOD.Studio.EVENT_CALLBACK dialogueCallback;


    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.createChannelGroup("cg", out channelGroup);
        for (int i = 0; i < 8; i++)
        {
            channel[i] = new FMOD.Channel();
            channel[i].setChannelGroup(channelGroup);
        }
    }

    public void LoadSound(string fpath)
    {
        snd = new FMOD.Sound();
        FMODUnity.RuntimeManager.CoreSystem.createSound(fpath, FMOD.MODE.CREATESAMPLE, out snd);
    }
    public void PlayMP3()
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(snd, channelGroup, false, out channel[0]);
        channelGroup.setVolume(scrSetting.Volume);
    }
    public void StopMP3()
    {
        channelGroup.isPlaying(out isplaying);
        if (isplaying)
        {
            channelGroup.stop();
        }
    }
    public void ReleaseMP3()
    {
        snd.release();
    }
    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        FMODUnity.RuntimeManager.CoreSystem.release();
#endif
    }
}
