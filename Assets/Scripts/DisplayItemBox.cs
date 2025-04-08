using UnityEngine;

public class DisplayItemBox : MonoBehaviour
{
    private ParticleSystem particleSystem;
    public GameObject displayBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void MakeItRain(int amountOfMoney)
    {
        SoundManager.PlaySource(GetComponent<AudioSource>());
        var particleEmission = particleSystem.emission;
        ParticleSystem.Burst burst = particleEmission.GetBurst(0);
        burst.cycleCount = Mathf.Max(1, (int)Mathf.Ceil((float)amountOfMoney / burst.count.constant));
        particleEmission.SetBurst(0, burst);
        particleSystem.Play();
    }

}
