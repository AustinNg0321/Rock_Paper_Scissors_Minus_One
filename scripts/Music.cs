using Godot;
using System;

public partial class Music : Node
{
    /* 
     * volume ranges from 0.0 to 1.0
     * the default volume is set to the maximum volume
     */
    private static float _volume = 1.0f;

    public static void PlayMusic(AudioStreamPlayer asp, AudioStreamWav music)
    {
        asp.SetStream(music);
        asp.Play();
    }

    public static void SetVolume(float volume)
    {
        _volume = volume;
    }

    public static float GetVolume()
    {
        return _volume;
    }

    public static void UpdateVolume()
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(_volume));
    }
}
