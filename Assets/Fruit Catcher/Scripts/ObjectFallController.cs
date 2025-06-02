using UnityEngine;

public class ObjectFallController : MonoBehaviour
{
    [Header("Pengaturan Jatuh")]
    public float fallSpeed = 2f; // Kecepatan jatuh buah
    public bool useGravity = false; // Apakah menggunakan gravity dari Rigidbody2D

    [Header("Pengaturan Efek")]
    public GameObject destroyEffect; // Prefab effect yang akan di-spawn saat buah dihancurkan
    public AudioClip catchSound; // Suara saat object dikumpulkan
    public AudioClip failSound; // Suara saat terkena obstacle

    [Header("Pengaturan Batas")]
    public float destroyYPosition = -10f; // Posisi Y dimana buah akan dihancurkan

    [Header("Pengaturan Jenis")]
    public bool isObstacle; // Apakah ini obstacle (mengakhiri permainan)
    public bool isDoubleScoreItem; // (jika ingin menambahkan power-up khusus)

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null && useGravity)
        {
            Debug.LogWarning("Rigidbody2D tidak ditemukan pada " + gameObject.name + " tapi useGravity diaktifkan!");
        }

        if (rb != null)
        {
            rb.gravityScale = useGravity ? 1f : 0f;
        }
    }

    void Update()
    {
        if (!useGravity)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        else if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
        }

        if (transform.position.y < destroyYPosition)
        {
            DestroyObject();
        }
    }

    // Method untuk menghancurkan objek
    private void DestroyObject()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, transform.rotation);
            Debug.Log("Effect di-spawn: " + destroyEffect.name + " di posisi buah");
        }

        Debug.Log("Buah " + gameObject.name + " dihancurkan karena jatuh terlalu jauh");
        Destroy(gameObject);
    }

    // Deteksi tabrakan
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Buah dikumpulkan oleh pemain!");
            
            // Coba ambil komponen PlayerMovement
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TriggerTongueOut(); // Aktifkan animasi lidah keluar
            }
            else
            {
                Debug.LogWarning("PlayerMovement tidak ditemukan di objek Player.");
            }


            // Jika ini obstacle, akhiri permainan
            if (isObstacle)
            {
                if (failSound != null)
                {
                    AudioManager.Instance.sfxSource.PlayOneShot(failSound, AudioManager.Instance.sfxVolume * 2.0f); // volume lebih kencang
                }

                Debug.Log("Obstacle tertangkap! Game Over!");
                GameManager.Instance.GameOver(); // Akhiri permainan
                Destroy(gameObject); // Hancurkan obstacle
                return; // Hindari eksekusi kode berikutnya
            }

            // Mainkan suara tangkap jika ada
            if (catchSound != null)
            {
                Debug.Log("Play catchSound: " + catchSound.name);
                AudioManager.Instance.PlaySFX(catchSound);
            }
            else
            {
                Debug.LogWarning("catchSound kosong!");
            }

            // Efek visual jika ada
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, transform.rotation);
                Debug.Log("Effect di-spawn: " + destroyEffect.name + " saat buah dikumpulkan");
            }

            // Tambah skor jika bukan obstacle
            if (GameManager.Instance != null)
            {
                int scoreToAdd = isDoubleScoreItem ? 2 : 1; // jika aktif maka double score
                GameManager.Instance.AddScore(scoreToAdd);
            }


            Destroy(gameObject); // Hancurkan objek setelah dikumpulkan
        }

        if (other.CompareTag("Ground"))
        {
            Debug.Log("Buah jatuh ke tanah!");
            DestroyObject();
        }
    }

    // Setter opsional dari luar script
    public void SetFallSpeed(float newSpeed)
    {
        if (newSpeed >= 0)
        {
            fallSpeed = newSpeed;
        }
    }

    public void SetUseGravity(bool useGrav)
    {
        useGravity = useGrav;
        if (rb != null)
        {
            rb.gravityScale = useGravity ? 1f : 0f;
        }
    }

    public void SetDestroyPosition(float yPos)
    {
        destroyYPosition = yPos;
    }
}
