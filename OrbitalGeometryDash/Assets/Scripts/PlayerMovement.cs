using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 10f;
    public int maxJumps = 1;
    public float movementSpeed = 10f; //units per second
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float rotationSpeed = 190; //degree per second
    
    [Space]
    public Transform playerArt;
    
    private Rigidbody2D rb2d;
    private int jumpsRemaining;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.linearVelocityX = movementSpeed;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && (IsGrounded() || jumpsRemaining > 0))
        {
            Jump();
        }
        else if (!IsGrounded())
        {
            playerArt.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpsRemaining--;
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
    
    void ClampRotation()
    {
        float zAngle = transform.rotation.eulerAngles.z % 90;
        playerArt.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - zAngle);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
