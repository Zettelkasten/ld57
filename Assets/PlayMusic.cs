using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public void Start()
    {
        // Play the music when the game starts
        SoundManager.Instance.PlayMusic(GetComponent<AudioSource>().clip);
    }
}
