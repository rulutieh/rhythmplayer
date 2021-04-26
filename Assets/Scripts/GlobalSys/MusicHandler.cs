using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    FMOD.ChannelGroup channelGroup = new FMOD.ChannelGroup();
    FMOD.Channel[] channel = new FMOD.Channel[8];
    FMOD.Sound snd;
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
        FMODUnity.RuntimeManager.CoreSystem.createSound(fpath, FMOD.MODE.CREATESAMPLE, out snd);
    }
    public void PlayMP3()
    {
        FMODUnity.RuntimeManager.CoreSystem.playSound(snd, channelGroup, false, out channel[0]);
        channelGroup.setVolume(scrSetting.Volume);
    }
    public void StopMP3()
    {
        channelGroup.stop();
    }
    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        FMODUnity.RuntimeManager.CoreSystem.release();
#endif
    }
}
