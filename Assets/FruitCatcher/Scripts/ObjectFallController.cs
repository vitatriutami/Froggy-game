using UnityEngine;

public class ObjectFallController : MonoBehaviour
{
    [Header("Pengaturan Jatuh")]
    public float fallSpeed = 3f; // Kecepatan jatuh buah
    public bool useGravity = false; // Apakah menggunakan gravity dari Rigidbody2D

    [Header("Pengaturan Efek")]
    public GameObject destroyEffect; // Prefab effect yang akan di-spawn saat buah dihancurkan
    public AudioClip catchSound; // Suara saat buah dikumpulkan

    [Header("Pengaturan Batas")]
    public float destroyYPosition = -10f; // Posisi Y dimana buah akan dihancurkan


    [Header("Pengaturan Jenis")]
    public bool isObstacle;
    public bool isDoubleScoreItem;

    private Rigidbody2D rb;

    void Start()
    {
        // Ambil komponen Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Jika tidak ada Rigidbody2D tapi menggunakan gravity, beri peringatan
        if (rb == null && useGravity)
        {
            Debug.LogWarning("Rigidbody2D tidak ditemukan pada " + gameObject.name + " tapi useGravity diaktifkan!");
        }

        // Atur gravityScale sesuai pengaturan
        if (rb != null)
        {
            rb.gravityScale = useGravity ? 1f : 0f;
        }
    }

    void Update()
    {
        // Jika tidak menggunakan gravity, gerakkan buah manual ke bawah
        if (!useGravity)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        // Jika menggunakan gravity, kontrol velocity Rigidbody
        else if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
        }

        // Jika buah jatuh melebihi batas bawah, hancurkan
        if (transform.position.y < destroyYPosition)
        {
            DestroyObject();
        }
    }

    // Method untuk mengatur kecepatan jatuh dari script lain
    public void SetFallSpeed(float newSpeed)
    {
        if (newSpeed >= 0)
        {
            fallSpeed = newSpeed;
        }
    }

    // Method untuk mengatur apakah menggunakan gravity
    public void SetUseGravity(bool useGrav)
    {
        useGravity = useGrav;

        if (rb != null)
        {
            rb.gravityScale = useGravity ? 1f : 0f;
        }
    }

    // Method untuk mengatur batas bawah penghancuran
    public void SetDestroyPosition(float yPos)
    {
        destroyYPosition = yPos;
    }

    // Method untuk menghancurkan buah (misal jatuh ke tanah atau terlalu jauh)
    private void DestroyObject()
    {
        // Jika ada efek visual, tampilkan
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, transform.rotation);
            Debug.Log("Effect di-spawn: " + destroyEffect.name + " di posisi buah");
        }

        Debug.Log("Buah " + gameObject.name + " dihancurkan karena jatuh terlalu jauh");
        Destroy(gameObject);
    }

    // Method yang dipanggil saat buah bertabrakan (dengan Player atau Ground)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Jika bertabrakan dengan pemain
        if (other.CompareTag("Player"))
        {
            Debug.Log("Buah dikumpulkan oleh pemain!");

            if (isObstacle)
            {
                GameManager.Instance.GameOver();
            }

            // 🔊 Putar suara di posisi ini
            if (catchSound != null)
            {
                AudioManager.Instance.PlaySFX(catchSound); // Volume lebih besar dari 1
            }

            // Spawn effect visual jika ada
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, transform.rotation);
                Debug.Log("Effect di-spawn: " + destroyEffect.name + " saat buah dikumpulkan");
            }

            // Tambahkan skor melalui GameManager
            if (GameManager.Instance != null && !isObstacle)
            {
                GameManager.Instance.AddScore(1);
            }

            Destroy(gameObject); // Hancurkan buah setelah ditangkap
        }

        // Jika bertabrakan dengan tanah
        if (other.CompareTag("Ground"))
        {
            Debug.Log("Buah jatuh ke tanah!");
            DestroyObject();
        }
    }
}
