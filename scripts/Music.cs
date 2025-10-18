using Godot;
using System;

public partial class Music : Node
{
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
