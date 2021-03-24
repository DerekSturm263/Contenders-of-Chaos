using System.Collections.Generic;
using UnityEngine;

public static class SoundPlayer
{
    public static AudioSource audioSource;
    public static List<AudioClip> soundList = new List<AudioClip>();
    public static float volume;

    // Used to set up the sound player.
    public static void Initialize()
    {
        Object[] soundListArray = Resources.LoadAll("Sounds/ui", typeof(AudioClip));
        foreach (var c in soundListArray)
        {
            Debug.Log("Sound Player: Added " + c.name + " to the Sound List.");
            soundList.Add(c as AudioClip);
        }

        audioSource = new GameObject("Sound Player").AddComponent<AudioSource>();
    }

    #region PlaySound Methods

    // Plays the current AudioClip
    public static void Play()
    {
        try
        {
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Sound Player is not playing any AudioClips.\nPerhaps you intended to use another Play method?");
        }
    }

    // Plays an AudioClip based on an AudioClip
    public static void Play(AudioClip c)
    {
        try
        {
            audioSource.clip = c;
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Sound Player does not contain a sound with the AudioClip of " + c + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on an index of the soundList array.
    public static void Play(int i)
    {
        try
        {
            audioSource.clip = soundList[i];
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Sound Player does not contain a sound at index " + i + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on a name as a string.
    public static void Play(string s)
    {
        try
        {
            foreach (AudioClip c in soundList)
            {
                if (c.name == s)
                {
                    audioSource.clip = c;
                    audioSource.Play();

                    break;
                }
            }
        }
        catch
        {
            Debug.LogError("The Sound Player does not contain a sound named " + s + "./nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    #endregion

    #region Return Sound AudioClips

    // Returns an AudioClip based on a name.
    public static AudioClip Sound(string s)
    {
        foreach (AudioClip c in soundList)
        {
            if (c.name == s)
                return c;
        }

        return null;
    }

    // Returns an AudioClip based on an int.
    public static AudioClip Sound(int i)
    {
        try
        {
            return soundList[i];
        }
        catch
        {
            Debug.LogError("There is no sound at index " + i + " of Sound Player.");
        }

        return null;
    }

    // Returns the AudioClip that's playing.
    public static AudioClip CurrentSound()
    {
        if (audioSource.isPlaying)
            return audioSource.clip;

        return null;
    }

    #endregion

    // Returns whether there's anything playing.
    public static bool isPlaying()
    {
        return audioSource.isPlaying;
    }

    // Changes the volume of the MusicPlayer.
    public static void ChangeVolume(float f)
    {
        volume = f;
        audioSource.volume = volume;
    }

    // Mutes the audio playback.
    public static void Mute()
    {
        volume = 0f;
        audioSource.volume = volume;
    }
}