using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTriggers : MonoBehaviour
{
    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("ClimbL1"))
        {
            player.canClimb = true;
            player.doubleJumpActive = false;
            player.dashActive = false;
        }
        if (collision.gameObject.name.Equals("JumpL2"))
        {
            player.doubleJumpActive = true;
            player.canClimb = false;
            player.dashActive = false;
        }
        if (collision.gameObject.name.Equals("DashL3"))
        {
            player.dashActive = true;
            player.canClimb = false;
            player.doubleJumpActive = false;
        }

        // Extra stuff for level 4 collision triggers, deal with later.

        if (collision.gameObject.name.Equals("LoadNextLevel"))
        {
            // Maybe play some sort of animation here.
            SceneManager.LoadScene("Level" + (int.Parse(SceneManager.GetActiveScene().name[5..]) + 1).ToString());
        }
    }
}
