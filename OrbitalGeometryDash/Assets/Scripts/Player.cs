using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class GeometryDashPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 10f;
    public int maxJumps = 1;
    public float movementSpeed = 360f; //units per second

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float rotationSpeed = 190; //degree per second

    [Header("Effects & Audio")]
    public ParticleSystem jumpEffect;
    public ParticleSystem deathEffect;
    public AudioSource jumpSound;
    public AudioSource deathSound;
    public Transform playerArt;

    private Rigidbody2D rb;
    private Animator anim;
    private int jumpsRemaining;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {

        if (isDead) return;

        transform.position += movementSpeed * Vector3.right * Time.deltaTime;
        if (Input.GetButtonDown("Jump") && (IsGrounded() || jumpsRemaining > 0))
        {
            Jump();
        }
        else if (!IsGrounded())
        {
            playerArt.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

    }

    private void FixedUpdate()
    {
        //put some physics here to help understand the purpose of this
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpsRemaining--;

        if (jumpEffect) jumpEffect.Play();
        if (jumpSound) jumpSound.Play();

        anim.SetTrigger("Jump");
    }

    private bool IsGrounded()
    {
        Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (col != null)
        {
            jumpsRemaining = maxJumps;
            ClampRotation();
            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spike"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        Die();
    }

    void ClampRotation()
    {
        float zAngle = transform.rotation.eulerAngles.z % 90;
        playerArt.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - zAngle);
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Death");

        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (deathSound) deathSound.Play();

        Invoke("Restart", 1f);
    }

    private void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}