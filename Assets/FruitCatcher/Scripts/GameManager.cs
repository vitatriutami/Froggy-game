using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static GameManager Instance;

    [Header("Pengaturan Game")]
    public float timeLimit = 60f; // Total waktu permainan dalam detik
    public int totalScore = 0; // Skor pemain
    public PlayerMovement player; // Referensi ke skrip player
    public AudioClip yeaySound; // Suara saat mode boost aktif
    public AudioClip bgmClip; // Musik latar belakang

    [Header("UI Elements")]
    public TMP_Text timeLimitText;
    public TMP_Text scoreText;
    public TMP_Text gameOverScoreText;
    public TMP_Text boostText;
    public TMP_Text endMessageText; // Teks akhir permainan
    public GameObject uiGameOver;

    [Header("Status Game")]
    public bool isGameOver = false;

    private float currentTime;
    private TextMeshProUGUI startText;

    // Boost Mode
    private bool isDoubleScoreAndSpeedActive = false;
    public float speedMultiplier = 1.5f;
    public int scoreMultiplier = 2;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = timeLimit;
        isGameOver = false;
        Time.timeScale = 1f;

        // Cari teks "Start!" dari scene
        GameObject startGO = GameObject.Find("Texts/Start");
        if (startGO != null)
        {
            startText = startGO.GetComponent<TextMeshProUGUI>();
        }

        if (uiGameOver != null)
        {
            uiGameOver.SetActive(false);
        }

        AudioManager.Instance?.PlayBGM(bgmClip);

        ValidateComponents();

        StartCoroutine(BootGameFlow());
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Aktifkan boost ketika waktu setengah
            if (!isDoubleScoreAndSpeedActive && currentTime <= timeLimit / 2f)
            {
                isDoubleScoreAndSpeedActive = true;
                player.SetSpeedMultiplier(speedMultiplier);
                AudioManager.Instance.sfxSource.PlayOneShot(yeaySound, AudioManager.Instance.sfxVolume * 1.9f);
                ShowBoostEffect();
            }

            // Kurangi waktu setiap frame
            currentTime -= Time.deltaTime;
            UpdateTimeUI();

            // Cek apakah waktu habis
            if (currentTime <= 0)
            {
                EndByTime();
            }
        }
    }

    // Coroutine untuk memulai game dengan efek teks "Start!"
    private IEnumerator BootGameFlow()
    {
        yield return StartCoroutine(ShowStartText());
        UpdateTimeUI();
        UpdateScoreUI();
    }

    private IEnumerator ShowStartText()
    {
        if (startText != null)
        {
            CanvasGroup canvasGroup = startText.GetComponent<CanvasGroup>() ?? startText.gameObject.AddComponent<CanvasGroup>();
            startText.text = "Start!";
            startText.gameObject.SetActive(true);

            float duration = 0.3f;
            float timer = 0f;

            // Fade in
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(0.6f);

            // Fade out
            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            startText.gameObject.SetActive(false);
        }
    }

    private void ShowBoostEffect()
    {
        if (boostText != null)
        {
            boostText.text = "DOUBLE MODE!";
            boostText.gameObject.SetActive(true);
            StartCoroutine(HideBoostTextAfterSeconds(1f));
        }

        Debug.Log("Mode BOOST aktif: Speed dan Score dobel!");
    }

    private IEnumerator HideBoostTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        boostText.gameObject.SetActive(false);
    }

    private void UpdateTimeUI()
    {
        if (timeLimitText != null)
        {
            int displayTime = Mathf.CeilToInt(currentTime);
            displayTime = Mathf.Max(displayTime, 0);
            timeLimitText.text = "Time: " + displayTime.ToString();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
    }

    // Tambahkan skor
    public void AddScore(int scoreToAdd = 1)
    {
        if (!isGameOver)
        {
            int finalScore = isDoubleScoreAndSpeedActive ? scoreToAdd * scoreMultiplier : scoreToAdd;
            totalScore += finalScore;
            UpdateScoreUI();

            Debug.Log("Skor ditambahkan: +" + scoreToAdd + " | Total Score: " + totalScore);
        }
    }

    // Dipanggil saat waktu habis — TAMAT
    public void EndByTime()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            player.gameObject.SetActive(false);
            if (uiGameOver != null) uiGameOver.SetActive(true);

            int finalScore = Mathf.Max(totalScore, 0);
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > highScore)
            {
                PlayerPrefs.SetInt("HighScore", finalScore);
                PlayerPrefs.Save();
                highScore = finalScore;
            }

            endMessageText.text = "GOOD JOB!";
            gameOverScoreText.text = $"Score: {finalScore}\nHigh Score: {highScore}";
            Debug.Log("Tamat! Skor Akhir: " + totalScore);
        }
    }

    // Dipanggil oleh obstacle (misal dari ObjectFallController.cs)
    public void GameOver(bool fromObstacle = true)
    {
        if (!isGameOver)
        {
            isGameOver = true;
            player.gameObject.SetActive(false);

            if (uiGameOver != null) uiGameOver.SetActive(true);

            int finalScore = Mathf.Max(totalScore, 0);
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > highScore)
            {
                PlayerPrefs.SetInt("HighScore", finalScore);
                PlayerPrefs.Save();
                highScore = finalScore;
            }

            endMessageText.text = "GAME OVER!";
            gameOverScoreText.text = $"Score: {finalScore}\nHigh Score: {highScore}";
            Debug.Log("Game Over karena obstacle. Skor Akhir: " + totalScore);
        }
    }

    // Reset permainan
    public void RestartGame()
    {
        isGameOver = false;
        isDoubleScoreAndSpeedActive = false;

        player.ResetSpeed();
        currentTime = timeLimit;
        totalScore = 0;
        Time.timeScale = 1f;

        if (uiGameOver != null) uiGameOver.SetActive(false);

        UpdateTimeUI();
        UpdateScoreUI();

        player.gameObject.SetActive(true);
        Debug.Log("Game direstart!");
    }

    // Tambahkan waktu bonus
    public void AddTime(float bonusTime)
    {
        if (!isGameOver)
        {
            currentTime += bonusTime;
            Debug.Log("Time Bonus: +" + bonusTime + " detik");
        }
    }

    public float GetRemainingTime() => currentTime;

    public int GetCurrentScore() => totalScore;

    private void ValidateComponents()
    {
        if (timeLimitText == null) Debug.LogWarning("Time Limit Text belum diatur pada GameManager!");
        if (scoreText == null) Debug.LogWarning("Score Text belum diatur pada GameManager!");
        if (uiGameOver == null) Debug.LogWarning("UI Game Over belum diatur pada GameManager!");
        if (gameOverScoreText == null) Debug.LogWarning("Game Over Score Text belum diatur!");
        if (endMessageText == null) Debug.LogWarning("End Message Text belum diatur!");
    }
}
