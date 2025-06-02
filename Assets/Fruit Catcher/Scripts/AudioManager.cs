using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Range(0f, 1f)] public float bgmVolume = 0.3f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-assign jika kosong
            var sources = GetComponents<AudioSource>();
            if (bgmSource == null && sources.Length > 0) bgmSource = sources[0];
            if (sfxSource == null && sources.Length > 1) sfxSource = sources[1];

            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    private void ApplyVolumes()
    {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }
}
