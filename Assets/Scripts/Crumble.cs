using UnityEngine;

public class Crumble : MonoBehaviour
{

    Rigidbody rb;
    BoxCollider2D box;
    SpriteRenderer sr;
    int frameCount = 0;
    int destrCount = 0;
    bool Collided = false;
    bool wasDestroyed = false;
    // Measures in seconds
    public int respawnTime;
    // Measures in seconds
    public float killTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

    }

    // Updates consistently 50 times per second.
    private void FixedUpdate()
    {
        // If the player collided with object or object was destroyed, begin respective counters.
        if (Collided)
        {
            frameCount++;
        }
        if (wasDestroyed)
        {
            destrCount++;
            // Check to respawn the platform every frame.
            respawnPlatform();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("GroundCheck"))
        {
            Collided = true;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (frameCount >= killTime)
        {
            // Turn off collisions, visibility, and reports to "destroy" the platform.
            box.enabled = false;
            sr.enabled = false;
            wasDestroyed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collided = false;
        frameCount = 0;
    }

    /// <summary>
    /// "Respawns" platform by returning collision/visibility after destrCount # of frames.
    /// </summary>
    void respawnPlatform()
    {
        if (wasDestroyed && destrCount >= (respawnTime * 50))
        {
            box.enabled = true;
            sr.enabled = true;
            wasDestroyed = false;
            destrCount = 0;
        }
    }

}