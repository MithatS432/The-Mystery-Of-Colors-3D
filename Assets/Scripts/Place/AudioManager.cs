using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource campfireSource;
    public AudioSource waterfallSource;
    public AudioSource sfxSource;

    public AudioClip campFireSound;
    public AudioClip waterfallSound;
    public AudioClip beeSound;

    void Start()
    {
        campfireSource.clip = campFireSound;
        campfireSource.loop = true;
        campfireSource.Play();

        waterfallSource.clip = waterfallSound;
        waterfallSource.loop = true;
        waterfallSource.Play();
    }

    public void PlayBeeSound()
    {
        sfxSource.PlayOneShot(beeSound);
    }
}
