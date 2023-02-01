using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private TextMeshProUGUI gameText;

    [SerializeField] private string[] playerDialogueSentences;

    // The box graphic
    [SerializeField] private GameObject textBox;
    // The words that appear
    [SerializeField] private GameObject theText;

    Collider2D currCollider;


    private int textPromptIndex;
    private int currPromptIndex;
    private bool isActive = false;
    private bool isWritingText = false;


    public void Start()
    {
        textPromptIndex = 0;
        currPromptIndex = 0;
    }

    public void Update()
    {
        if (textBox.activeSelf && !isActive && !isWritingText)
        {
            Debug.Log("isActive = true");
            new WaitForSeconds(0.8f);
            StartCoroutine(TypeGameDialogue());
            isActive = true;
        }

        if (Input.GetKeyDown(KeyCode.Return) && isActive == true)
        {
            Debug.Log("set box active = false");
            gameText.text = "";
            textBox.SetActive(false);
            theText.SetActive(false);
            isActive = false;

            if (currCollider != null)
            {
                Destroy(currCollider);
            }
        }
    }

    private IEnumerator TypeGameDialogue()
    {
        isWritingText = true;
        foreach (char letter in playerDialogueSentences[textPromptIndex].ToCharArray())
        {
            if (currCollider != null)
            {
                if (currCollider.gameObject.name.Contains("TextPrompt_" + textPromptIndex))
                {
                    gameText.text += letter;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
        }
        isWritingText = false;
        textPromptIndex++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("TextPrompt_" + currPromptIndex))
        {
            Debug.Log("collided " + currPromptIndex);
            currPromptIndex++;
            if (isActive && currCollider != null)
            {
                Debug.Log("destroy collider, clear text");

                // Clear text from the text box and remove collider
                gameText.text = "";
                Destroy(currCollider);
                textBox.SetActive(false);
                theText.SetActive(false);
                isActive = false;
            }
            
            Debug.Log("set box active = true");
            // Make text and box visible
            currCollider = collision;
            textBox.SetActive(true);
            theText.SetActive(true);
        }
    }
}
