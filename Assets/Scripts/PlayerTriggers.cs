using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTriggers : MonoBehaviour
{
    private PlayerMovement player;
    private bool level4ClimbUsed = false;
    private bool level4JumpUsed = false;
    private bool level4DashUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.BackQuote))
        {
            SceneManager.LoadScene("Level" + (int.Parse(SceneManager.GetActiveScene().name.Substring(5)) + 1).ToString());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Levels 1-3
        if (collision.gameObject.name.Equals("ClimbL1"))
        {
            player.canClimb = true;
            player.doubleJumpActive = false;
            player.dashActive = false;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.name.Equals("JumpL2"))
        {
            player.doubleJumpActive = true;
            player.canClimb = false;
            player.dashActive = false;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.name.Equals("DashL3"))
        {
            player.dashActive = true;
            player.canClimb = false;
            player.doubleJumpActive = false;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.name.Equals("LoadNextLevel"))
        {
            SceneManager.LoadScene("Level" + (int.Parse(SceneManager.GetActiveScene().name[5..]) + 1).ToString());
        }


        // Level 4.
        if (collision.gameObject.name.Equals("ClimbL4"))
        {
            player.canClimb = true;
            player.doubleJumpActive = false;
            player.dashActive = false;
            level4ClimbUsed = true;

            // Set the all triggers related to level completion to their correct state.
            Level4Triggers[] triggers = collision.gameObject.GetComponent<Transform>().parent.GetComponentsInChildren<Level4Triggers>();
            for (int i = 1; i < triggers.Length; i++)
            {
                if (triggers[i].name == "ClimbL4" || triggers[i].name == "JumpL4" || triggers[i].name == "DashL4")
                {
                    triggers[i].SetActive(false);
                }
                else
                {
                    triggers[i].SetActive(true);
                }
            }
        }
        if (collision.gameObject.name.Equals("JumpL4"))
        {
            player.doubleJumpActive = true;
            player.canClimb = false;
            player.dashActive = false;
            level4JumpUsed = true;

            // Set the all triggers related to level completion to their correct state.
            Level4Triggers[] triggers = collision.gameObject.GetComponent<Transform>().parent.GetComponentsInChildren<Level4Triggers>();
            for (int i = 1; i < triggers.Length; i++)
            {
                if (triggers[i].name == "ClimbL4" || triggers[i].name == "JumpL4" || triggers[i].name == "DashL4")
                {
                    triggers[i].SetActive(false);
                }
                else
                {
                    triggers[i].SetActive(true);
                }
            }
        }
        if (collision.gameObject.name.Equals("DashL4"))
        {
            player.dashActive = true;
            player.canClimb = false;
            player.doubleJumpActive = false;
            level4DashUsed = true;

            // Set the all triggers related to level completion to their correct state.
            Level4Triggers[] triggers = collision.gameObject.GetComponent<Transform>().parent.GetComponentsInChildren<Level4Triggers>();
            for (int i = 1; i < triggers.Length; i++)
            {
                if (triggers[i].name == "ClimbL4" || triggers[i].name == "JumpL4" || triggers[i].name == "DashL4" || triggers[i].name == "BackwardPassedTrigger")
                {
                    triggers[i].SetActive(false);
                }
                else
                {
                    triggers[i].SetActive(true);
                }
            }
        }

        if (collision.gameObject.name.Equals("LoadCredits"))
        {
            if (level4ClimbUsed && level4JumpUsed && level4DashUsed)
            {
                SceneManager.LoadScene("Credits");
            }

            Level4Triggers[] triggers = collision.gameObject.GetComponent<Transform>().parent.GetComponentsInChildren<Level4Triggers>();
            for (int i = 1; i < triggers.Length; i++)
            {
                if (triggers[i].name == "ClimbL4" && !level4ClimbUsed)
                {
                    triggers[i].SetActive(true); ;
                }
                else if (triggers[i].name == "JumpL4" && !level4JumpUsed)
                {
                    triggers[i].SetActive(true);
                }
                else if (triggers[i].name == "DashL4" && !level4DashUsed)
                {
                    triggers[i].SetActive(true);
                }
                else if (triggers[i].name == "PreventBackwards")
                {
                    triggers[i].SetActive(false);
                }
            }
        }

        if (collision.gameObject.name.Equals("BackwardPassedTrigger"))
        {
            Level4Triggers[] triggers = collision.gameObject.GetComponent<Transform>().parent.GetComponentsInChildren<Level4Triggers>();
            for (int i = 1; i < triggers.Length; i++)
            {
                if (triggers[i].name == "PreventBackwards")
                {
                    triggers[i].SetActive(true); ;
                }
            }
        }
    }
}
