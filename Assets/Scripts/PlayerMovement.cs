using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Ability activation booleans.
    private bool canClimb = false; // Climb is holding down left control
    private bool doubleJumpActive = false;
    private bool dashActive = false;

    // Regular movement variables.
    private Rigidbody2D rb;

    [SerializeField] private float movementSpeed = 8f;

    public float gravityScale = 5.5f;
    [SerializeField] private float jumpSpeed = 12f;

    [SerializeField] private float airDecelerationFactor = 1.01f;
    [SerializeField] private float groundDecelerationFactor = 1.5f;

    private bool isFacingRight = true;

    private bool movingLeft = true;
    private bool movingRight = true;

    // Climb movement variables.
    private bool climbing = false;
    [SerializeField] private float climbingSpeed = 5;
    private bool movingUp = false;
    private bool movingDown = false;
    private int clingJumpTime = 0;
    [SerializeField] private int clingJumpDuration = 5;
    [SerializeField] private float clingJumpSpeed = 15f;

    // Jump movement variables.
    private bool grounded = false;

    [SerializeField] private float coyoteTime = .15f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = .1f;
    private float jumpBufferCounter;
    private bool jumping = false;
    [SerializeField] private float jumpDecelerationFactor = 0.5f;

    private bool canDoubleJump = false;
    private bool doubleJumping = false;
    [SerializeField] private float doubleJumpSpeed = 15f;

    // Dash movement variables.
    private int dashTime = 0;
    [SerializeField] private int dashDuration = 15;
    [SerializeField] private int dashStallTime = 5;
    private bool dashInput = false;
    [SerializeField] private int dashSpeed = 50;
    private bool dashInputReleased = true;
    private bool canDash = false;

    // Animation
    private Animator playerAnimator;

    // Layer and collision check variables.
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheckLeft;
    [SerializeField] Transform wallCheckRight;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        movingLeft = false;
        movingRight = false;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashInput = true;
        }

        if (!Input.GetKeyUp(KeyCode.LeftShift))
        {
            dashInputReleased = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && climbing)
        {
            clingJumpTime = clingJumpDuration;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            playerAnimator.SetTrigger("isJumping");

            jumping = true;

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded() && canDoubleJump)
        {
            doubleJumping = true;
            canDoubleJump = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) && canClimb && TouchingWall())
        {
            climbing = true;
            rb.gravityScale = 0;
        }

        if (!TouchingWall())
        {
            climbing = false;
            rb.gravityScale = gravityScale;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            climbing = false;
            movingUp = false;
            movingDown = false;
            rb.gravityScale = gravityScale;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && climbing)
        {
            movingUp = true;
        }

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && climbing)
        {
            movingDown = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            movingUp = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            movingDown = false;
        }
    }

    private void FixedUpdate()
    {
        if ((movingLeft || movingRight) && dashTime == 0)
        {
            if (!movingRight)
            {
                rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
            }
            else if (!movingLeft)
            {
                rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
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
                rb.velocity = new Vector2((float)(rb.velocity.x / airDecelerationFactor), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2((float)(rb.velocity.x / groundDecelerationFactor), rb.velocity.y);
            }
        }

        if (climbing && clingJumpTime == 0)
        {
            if (movingUp)
            {
                rb.velocity = new Vector2(0, climbingSpeed);
            }
            else if (movingDown)
            {
                rb.velocity = new Vector2(0, -climbingSpeed);
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }

        if (clingJumpTime > 0)
        {
            clingJumpTime--;
            if (TouchingWall())
            {
                rb.velocity = new Vector2(rb.velocity.x, clingJumpSpeed);
            }
        }

        Debug.Log("dashInput " + dashInput);
        Debug.Log("canDash " + canDash);
        Debug.Log("dashInputReleased " + dashInputReleased);

        if (dashInput && canDash && dashInputReleased)
        {
            climbing = false;
            canDash = false;
            dashInput = false;
            TriggerDash();
            dashInputReleased = false;
        }

        if (dashTime > dashDuration - dashStallTime)
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
            if (grounded && dashActive)
            {
                canDash = true;
            }

            if (!climbing)
            {
                rb.gravityScale = gravityScale;
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
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            jumpBufferCounter = 0;
        }

        if (jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpDecelerationFactor);
            playerAnimator.SetTrigger("isJumping");
        }

        if (doubleJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpSpeed);
            playerAnimator.SetTrigger("isJumping");
            doubleJumping = false;
        }

        Flip();
        playerAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.02f, groundLayer);
    }

    public bool TouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckLeft.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(wallCheckRight.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if ((isFacingRight && rb.velocity.x < -.1f) || (!isFacingRight && rb.velocity.x > .1f) || (isFacingRight && movingLeft && !movingRight) || (!isFacingRight && movingRight && !movingLeft))
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

        dashTime = dashDuration;
    }

    /// <summary>
    // If ability has been triggered, set respective flags to true:
    /// - canClimb: "Climb"
    /// - doubleJumpActive: "Jump"
    /// - canDash: "Dash"
    /// </summary>
    /// <param name="collision">collision box</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Climb"))
        {
            canClimb = true;
            doubleJumpActive = false;
            dashActive = false;
        }
        if (collision.gameObject.name.Equals("Jump"))
        {
            doubleJumpActive = true;
            canClimb = false;
            dashActive = false;
        }
        if (collision.gameObject.name.Equals("Dash"))
        {
            dashActive = true;
            canClimb = false;
            doubleJumpActive = false;
        }

        if (collision.gameObject.name.Equals("LoadNextLevel"))
        {
            // Maybe play some sort of animation here.
            SceneManager.LoadScene("Level" + (int.Parse(SceneManager.GetActiveScene().name.Substring(5)) + 1).ToString());
        }
    }

    public void SetCanDash(bool canDash)
    {
        this.canDash = canDash;
    }
}
