using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private TextMeshProUGUI gameText;

    [SerializeField] private string[] playerDialogueSentences;

    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject theText;


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


        if (Input.GetKey(KeyCode.Return))
        {
            textBox.SetActive(false);
            theText.SetActive(false);
            isActive = false;
        }
    }

    private IEnumerator TypeGameDialogue()
    {
        foreach(char letter in playerDialogueSentences[textPromptIndex].ToCharArray())
        {
            gameText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Console.WriteLine(collision.gameObject.name);
        if (collision.gameObject.name == "TextPrompt_1")
        {
            textBox.SetActive(true);
            theText.SetActive(true);
            textPromptIndex = 0;
        }

        if (collision.name == "TextPrompt_2")
        {

        }

        if (collision.name == "TextPrompt_3")
        {

        }
    }
}
