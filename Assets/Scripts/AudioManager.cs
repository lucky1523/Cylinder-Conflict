using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayAudioSFX(AudioClip audioclip, float volume)
    {
        StartCoroutine(PlayAudio(audioclip, volume));
    }

    IEnumerator PlayAudio(AudioClip audioclip, float volume = 1f)
    {
        AudioSource audiosource = gameObject.AddComponent<AudioSource>();
        audiosource.clip = audioclip;
        audiosource.volume = volume;
        audiosource.Play();

        yield return new WaitForSeconds(1f * audioclip.length);
        Destroy(audiosource);
    }
}
