using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Ability activation booleans.
    private bool canClimb = true;
    private bool doubleJumpActive = true;
    private bool canDash = true;

    // Other needed movement variables.
    private int dashTime = 0;
    private bool dashInput = false;
    private int dashSpeed = 30;
    private bool dashInputReleased = true;

    private bool grounded = false;

    private float coyoteTime = .15f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = .1f;
    private float jumpBufferCounter;
    private bool jumping = false;

    private bool canDoubleJump = false;
    private bool doubleJumping = false;

    private bool isFacingRight = true;

    private bool movingLeft = true;
    private bool movingRight = true;

    private Rigidbody2D rb;

    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movingLeft = false;
        movingRight = false;
        dashInput = false;
        jumping = false;

        grounded = IsGrounded();

        if (grounded && doubleJumpActive)
        {
            canDoubleJump = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movingLeft = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movingRight = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dashInput = true;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            dashInputReleased = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            jumping = true;

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded() && canDoubleJump)
        {
            doubleJumping = true;
            canDoubleJump = false;
        }
    }

    private void FixedUpdate()
    {
        if (dashInput && canDash && dashInputReleased)
        {
            canDash = false;
            dashInput = false;
            TriggerDash();
            dashInputReleased = false;
        }

        if (dashTime > 5)
        {
            dashTime--;
            rb.velocity = new Vector2(rb.velocity.x, 0);

            return;
        }
        else if (dashTime > 0)
        {
            dashTime--;
            rb.velocity = new Vector2(rb.velocity.x / dashSpeed, 0);

            return;
        }

        if (dashTime == 0)
        {
            if (grounded)
            {
                canDash = true;
            }

            rb.gravityScale = 10f;
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
        else if (dashTime == 0)
        {
            if (!grounded)
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.01), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / 1.2), rb.velocity.y);
            }
        }

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 15f);

            jumpBufferCounter = 0;
        }

        if (jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (doubleJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, 25f);
            doubleJumping = false;
        }

        Flip();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.02f, groundLayer);
    }

    private void Flip()
    {
        if ((isFacingRight && rb.velocity.x < -.1f) || (!isFacingRight && rb.velocity.x > .1f))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void TriggerDash()
    {
        rb.gravityScale = 0f;

        if (isFacingRight)
        {
            rb.velocity = new Vector2(dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-dashSpeed, 0);
        }

        dashTime = 10;
    }
}
