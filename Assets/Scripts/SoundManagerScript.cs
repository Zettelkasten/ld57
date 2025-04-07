using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip, float pitch = 1)
    {
        EffectsSource.pitch = pitch;
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    // Play a single clip through the music source.
    public bool PlayMusic(AudioClip clip)
    {
        if (MusicSource.clip == clip)
        {
            return false;
        }
        MusicSource.clip = clip;
        MusicSource.Play();
        return true;
    }

    public void SetMusicVolume(float volume)
    {
        MusicSource.volume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        EffectsSource.volume = volume;
    }

    public static void PlaySource(AudioSource source)
    {
        if (Instance == null)
        {
            // fallback
            source.Play();
        }
        else
        {
            source.volume = Instance.EffectsSource.volume;
            source.Play();
        }
    }
}