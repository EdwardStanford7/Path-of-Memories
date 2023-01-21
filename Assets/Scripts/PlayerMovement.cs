using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float coyoteTime = .15f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = .1f;
    private float jumpBufferCounter;

    private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Animator mouseAnimator;

    //[SerializeField] public GameController controller;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        //GetComponent<Transform>().position = controller.spawnPoint;
        rb = GetComponent<Rigidbody2D>();
        mouseAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                mouseAnimator.SetTrigger("getKnock");
            }
        }
    }
    void FixedUpdate()
    {
        bool movingLeft = false;
        bool movingRight = false;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movingLeft = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movingRight = true;
        }

        if (movingLeft || movingRight)
        {
            if (!movingRight)
            {
                rb.velocity = new Vector2(-10, rb.velocity.y);
            }
            else if (!movingLeft)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            if (!IsGrounded())
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.01), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.2), rb.velocity.y);
            }
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 20f);

            jumpBufferCounter = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            mouseAnimator.SetTrigger("isJumping");

            coyoteTimeCounter = 0f;
        }

        Flip();
        mouseAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.02f, groundLayer);
    }

    private void Flip()
    {
        if ((isFacingRight && rb.velocity.x < 0f) || (!isFacingRight && rb.velocity.x > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void Die()
    {
        // Play death animation probably
        //transform.position = controller.spawnPoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            SceneManager.LoadScene(3);
        }
    }
}