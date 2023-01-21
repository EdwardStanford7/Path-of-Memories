using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private bool PlayerSpeakingFirst;

    [SerializeField] private TextMeshProUGUI playerDialogueText;

    [SerializeField] private string[] playerDialogueSentences;

    [SerializeField] private GameObject playerSpeechBubble;

    [TextArea]
    [SerializeField] private GameObject playerContinueButton;

    private int playerIndex;
    private bool isActive = false;


    public void Start()
    {
        playerSpeechBubble.SetActive(true);
    }

    public void Update()
    {
        if (playerSpeechBubble.activeSelf && !isActive)
        {
            StartCoroutine(TypePlayerDialogue());
            isActive = true;
        }
    }

    private IEnumerator TypePlayerDialogue()
    {
        foreach(char letter in playerDialogueSentences[playerIndex].ToCharArray())
        {
            playerDialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        playerContinueButton.SetActive(true);
    }
}
