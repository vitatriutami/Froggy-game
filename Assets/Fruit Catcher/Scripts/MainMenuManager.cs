using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Untuk EventTrigger opsional
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Start()
    {
        // ✅ Play background music jika ada
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();

        // ✅ Tampilkan High Score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    public void OnPlayButtonPressed()
    {
        PlayClickSFX();
        SceneManager.LoadScene("GameScene");
    }

    public void PlayHoverSFX()
    {
        if (sfxSource != null && hoverSound != null)
            sfxSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSFX()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }
}
