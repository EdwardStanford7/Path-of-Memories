using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private TextMeshProUGUI gameText;

    [SerializeField] private string[] playerDialogueSentences;

    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject theText;

    Collider2D currCollider;


    private int textPromptIndex;
    private bool isActive = false;


    public void Start()
    {
    }

    public void Update()
    {
        if (textBox.activeSelf && !isActive)
        {
            StartCoroutine(TypeGameDialogue());
            isActive = true;
        }

        if (Input.GetKey(KeyCode.Return) && isActive == true)
        {
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
        gameText.text = "";
        foreach(char letter in playerDialogueSentences[textPromptIndex].ToCharArray())
        {
            gameText.text += letter;
            if (currCollider != null)
            {
                if (currCollider.gameObject.name.Contains("TextPrompt_" + textPromptIndex))
                {
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
        }
        textPromptIndex++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("TextPrompt_" + textPromptIndex))
        {
            if (isActive && currCollider != null)
            {
                new WaitForSeconds(0.7f);
                    Destroy(currCollider);
                    textBox.SetActive(false);
                    theText.SetActive(false);
                    isActive = false;
            }
            currCollider = collision;
            textBox.SetActive(true);
            theText.SetActive(true);
        }
    }
}
