using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pengaturan Gerakan")]
    public float moveSpeed = 5f;

    [Header("Komponen")]
    public Rigidbody2D rigidBody;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool isWalking;
    private float originalSpeed;

    void Start()
    {
        // Cek apakah komponen ada
        if (rigidBody == null)
        {
            Debug.LogError("Komponen Rigidbody2D tidak ditemukan pada " + gameObject.name);
            rigidBody = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            Debug.LogError("Komponen Animator tidak ditemukan pada " + gameObject.name);
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("Komponen SpriteRenderer tidak ditemukan pada " + gameObject.name);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // balik ke kecepatan awal saat di-restart
        originalSpeed = moveSpeed;
    }

    void Update()
    {
        // Ambil input horizontal (tombol A/D atau panah kiri/kanan)
        horizontalInput = Input.GetAxis("Horizontal");

        // Cek apakah pemain sedang bergerak
        isWalking = Mathf.Abs(horizontalInput) > 0.1f;

        // Update animator
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }

        // Balik sprite karakter berdasarkan arah gerakan
        if (spriteRenderer != null)
        {
            if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false; // Menghadap kanan
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true; // Menghadap kiri
            }
        }
    }

    void FixedUpdate()
    {
        // Gerakkan karakter menggunakan Rigidbody2D
        if (rigidBody != null)
        {
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rigidBody.velocity.y);
            rigidBody.velocity = movement;
        }
    }

    // Method untuk tambah kecepatan
    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed *= multiplier;
    }

public void ResetSpeed()
{
    moveSpeed = originalSpeed;
}

}

