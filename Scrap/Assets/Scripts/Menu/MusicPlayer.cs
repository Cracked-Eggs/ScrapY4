using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip introCLip;
    [SerializeField] AudioClip mainClip;
    [SerializeField] AudioSource audioSource;
    
    public bool playOnAwake = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.loop = false;
        audioSource.clip = introCLip;

        if (playOnAwake)
            PlaySong();
    }

    public void PlaySong()
    {
        audioSource.PlayOneShot(introCLip);

        audioSource.clip = mainClip;
        audioSource.PlayScheduled(AudioSettings.dspTime + introCLip.length);
        audioSource.loop = true;
    }
}
