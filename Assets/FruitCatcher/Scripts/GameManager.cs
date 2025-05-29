using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static GameManager Instance;

    [Header("Pengaturan Game")]
    public float timeLimit = 60f; // Batas waktu dalam detik
    public int totalScore = 0; // Total skor pemain
    public PlayerMovement player;

    [Header("UI Elements")]
    public TMP_Text timeLimitText; // Text untuk menampilkan waktu
    public TMP_Text scoreText; // Text untuk menampilkan skor
    public TMP_Text gameOverScoreText; //Text untuk menampilkan skor pada saat game over
    public GameObject uiGameOver; // UI Game Over yang akan diaktifkan
    public TMP_Text boostText; // atau gunakan Text biasa kalau tidak pakai TMP


    [Header("Status Game")]
    public bool isGameOver = false;

    private float currentTime;

    void Awake()
    {
        // Implementasi Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hancurkan saat load scene baru
        }
        else
        {
            Destroy(gameObject); // Hancurkan duplikat
            return;
        }
    }
    public AudioClip bgmClip;
    private TextMeshProUGUI startText;
    private IEnumerator ShowStartText()
{
    if (startText != null)
    {
        CanvasGroup canvasGroup = startText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = startText.gameObject.AddComponent<CanvasGroup>();
        }

        startText.text = "Start!";
        startText.gameObject.SetActive(true);

        // Fade-in
        float duration = 0.3f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Tahan selama 1 detik
        yield return new WaitForSeconds(0.6f);

        // Fade-out
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

    private IEnumerator BootGameFlow()
{
    // Tampilkan teks “Start!” selama 2 detik
    yield return StartCoroutine(ShowStartText());

    // Setelah itu baru update UI
    UpdateTimeUI();
    UpdateScoreUI();

    // logika lain bisa dimasukkan di sini
}


    void Start()
{
    // Inisialisasi awal
    currentTime = timeLimit;
    isGameOver = false;
    Time.timeScale = 1f;

    // Cari GameObject "Start"
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

    // Tunda update UI dan logika game sampai setelah “Start!” ditampilkan
    StartCoroutine(BootGameFlow());
}

    // untuk mendapatkan double score
private bool isDoubleScoreAndSpeedActive = false;
public float speedMultiplier = 1.5f;
public int scoreMultiplier = 2;

private void ShowBoostEffect()
{
    if (boostText != null)
    {
        boostText.text = "DOUBLE MODE!";
        boostText.gameObject.SetActive(true);

        // Hilangkan setelah 2 detik
        StartCoroutine(HideBoostTextAfterSeconds(1f));
    }

    Debug.Log("Mode BOOST aktif: Speed dan Score dobel!");
}

private IEnumerator HideBoostTextAfterSeconds(float seconds)
{
    yield return new WaitForSeconds(seconds);
    boostText.gameObject.SetActive(false);
}

    void Update()
    {
        // Jika waktu tinggal setengah dan belum aktifkan boost
        if (!isDoubleScoreAndSpeedActive && currentTime <= timeLimit / 2f)
        {
            isDoubleScoreAndSpeedActive = true;

            // Aktifkan speed multiplier ke player
            player.SetSpeedMultiplier(speedMultiplier);

            // Aktifkan score multiplier
            Debug.Log("Mode BOOST aktif: Speed x" + speedMultiplier + ", Score x" + scoreMultiplier);

            // Tampilkan visual "DOUBLE MODE!" dan efek suara
            ShowBoostEffect();
        }
        // Jika game belum berakhir, hitung mundur waktu
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;

            // Update UI waktu
            UpdateTimeUI();

            // Cek apakah waktu sudah habis
            if (currentTime <= 0)
            {
                GameOver();
            }
        }

        
    }

    private void UpdateTimeUI()
    {
        if (timeLimitText != null)
        {
            // Bulatkan waktu ke integer
            int displayTime = Mathf.CeilToInt(currentTime);

            // Pastikan tidak menampilkan waktu negatif
            if (displayTime < 0)
            {
                displayTime = 0;
            }

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

    public void AddScore(int scoreToAdd = 1)
    {
        if (!isGameOver)
        {
            int finalScore = isDoubleScoreAndSpeedActive ? scoreToAdd * scoreMultiplier : scoreToAdd;
totalScore += finalScore;

            UpdateScoreUI();
            Debug.Log("Skor ditambahkan: +" + scoreToAdd + " | Total Skor: " + totalScore);
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            // Pause game

            int displayTime = Mathf.CeilToInt(currentTime);

            if (displayTime < 0)
            {
                displayTime = 0;
            }

            timeLimitText.text = "Time: " + displayTime.ToString();

            player.gameObject.SetActive(false);
            // Aktifkan UI Game Over
            if (uiGameOver != null)
            {
                uiGameOver.SetActive(true);
            }

            gameOverScoreText.text = "Total Score: " + totalScore.ToString();


            Debug.Log("Game Over! Skor Akhir: " + totalScore);
        }
    }

    // Method untuk restart game
    public void RestartGame()
    {
        isGameOver = false;
        isDoubleScoreAndSpeedActive = false;
player.ResetSpeed(); // Tambahkan method ini di PlayerMovement

        currentTime = timeLimit;
        totalScore = 0;
        Time.timeScale = 1f;

        if (uiGameOver != null)
        {
            uiGameOver.SetActive(false);
        }

        UpdateTimeUI();
        UpdateScoreUI();

        Debug.Log("Game direstart!");
        player.gameObject.SetActive(true);
    }

    // Method untuk menambah waktu (bonus waktu)
    public void AddTime(float bonusTime)
    {
        if (!isGameOver)
        {
            currentTime += bonusTime;
            Debug.Log("Time Bonus: +" + bonusTime + " detik");
        }
    }

    // Method untuk mendapatkan waktu sisa
    public float GetRemainingTime()
    {
        return currentTime;
    }

    // Method untuk mendapatkan skor saat ini
    public int GetCurrentScore()
    {
        return totalScore;
    }



    // Method untuk validasi komponen
    private void ValidateComponents()
    {
        if (timeLimitText == null)
        {
            Debug.LogWarning("Time Limit Text belum diatur pada GameManager!");
        }

        if (scoreText == null)
        {
            Debug.LogWarning("Score Text belum diatur pada GameManager!");
        }

        if (uiGameOver == null)
        {
            Debug.LogWarning("UI Game Over belum diatur pada GameManager!");
        }
    }
}