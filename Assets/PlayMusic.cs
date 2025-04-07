using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public void Start()
    {
        // Play the music when the game starts
        SoundManager.Instance.PlayMusic(GetComponent<AudioSource>().clip);
        Debug.Log("Playing music: " + GetComponent<AudioSource>().clip.name);
    }

    public void Update()
    {
        Debug.Log("Music is playing: " + GetComponent<AudioSource>().isPlaying);
    }
}
