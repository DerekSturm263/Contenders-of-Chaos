using System.Collections.Generic;
using UnityEngine;

public static class MusicPlayer
{
    public static AudioSource audioSource;
    public static List<AudioClip> trackList = new List<AudioClip>();
    public static float volume;

    // Used to set up the music player.
    public static void Initialize()
    {
        Object[] trackListArray = Resources.LoadAll("Audio/Music/", typeof(AudioClip));
        foreach (var c in trackListArray)
        {
            Debug.Log("Music Player: Added " + c.name + " to the Track List.");
            trackList.Add(c as AudioClip);
        }

        audioSource = new GameObject("Music Player").AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    #region PlayTrack Methods

    public static bool Exists()
    {
        return GameObject.FindObjectOfType<AudioSource>();
    }

    // Plays the current AudioClip.
    public static void Play()
    {
        try
        {
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Music Player is not playing any AudioClips.\nPerhaps you intended to use another Play method?");
        }
    }

    // Plays an AudioClip based on an AudioClip.
    public static void Play(AudioClip c)
    {
        try
        {
            audioSource.clip = c;
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Music Player does not contain a track with the AudioClip of " + c + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on an index of the trackList array.
    public static void Play(int i)
    {
        try
        {
            audioSource.clip = trackList[i];
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("The Music Player does not contain a track at index " + i + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on a name as a string.
    public static void Play(string s)
    {
        try
        {
            foreach (AudioClip c in trackList)
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
            Debug.LogError("The Music Player does not contain a track named " + s + "./nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    #endregion

    #region Return Track AudioClips

    // Returns an AudioClip based on a name.
    public static AudioClip Track(string s)
    {
        foreach (AudioClip c in trackList)
        {
            if (c.name == s)
                return c;
        }

        return null;
    }

    // Returns an AudioClip based on an int.
    public static AudioClip Track(int i)
    {
        try
        {
            return trackList[i];
        }
        catch
        {
            Debug.LogError("There is no track at index " + i + " of Music Player.");
        }

        return null;
    }

    // Returns the AudioClip that's playing.
    public static AudioClip CurrentTrack()
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

    #region Playback Controls

    // Stops the music playback and removes the current AudioClip.
    public static void Pause()
    {
        audioSource.Stop();
    }

    // Stops the music playback and removes the current AudioClip.
    public static void Stop()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    // Restarts the current track.
    public static void Restart()
    {
        audioSource.Stop();
        audioSource.Play();
    }

    // Goes to the next track.
    public static void Next()
    {
        try
        {
            audioSource.clip = trackList[trackList.IndexOf(audioSource.clip) + 1];
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("There is no track after " + audioSource.clip.name + " in the Track List.");
        }
    }

    // Goes to the previous track.
    public static void Previous()
    {
        try
        {
            audioSource.clip = trackList[trackList.IndexOf(audioSource.clip) - 1];
            audioSource.Play();
        }
        catch
        {
            Debug.LogError("There is no track before " + audioSource.clip.name + " in the Track List.");
        }
    }

    #endregion
}