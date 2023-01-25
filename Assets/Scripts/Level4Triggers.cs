using UnityEngine;

public class Level4Triggers : MonoBehaviour
{
    public void SetActive(bool active)
    {
        GetComponent<BoxCollider2D>().enabled = active;
        GetComponent<SpriteRenderer>().enabled = active;
    }
}
