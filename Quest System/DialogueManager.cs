﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> dialogueQueue;
    private PlayerMovement playerMovement;
    public GameObject playerDialogueBox;
    private QuestManager questManager;
    private TMPro.TextMeshProUGUI textMeshPro;
    internal bool dialogueInitiated;
    private List<string> questProductList;

    #region Init Functions

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        dialogueQueue = new Queue<string>();
        textMeshPro = playerDialogueBox.GetComponent<TMPro.TextMeshProUGUI>();
        dialogueInitiated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueInitiated && Input.GetKeyDown(KeyCode.Space)) // indicate on the UI that player has to press space to progress in the dialogue
        {
            DisplayNextDialogue();
        }
    }
    #endregion

    #region Internal Functions

    internal void DisplayInstruction(bool activate, bool setInstruction)
    {
        if(setInstruction)
        textMeshPro.SetText("Press R to talk");

        playerDialogueBox.SetActive(activate);
    }

    internal void StartQuestDialogue(Dialogue dialogue, List<string> requiredProducts)
    {
        dialogueQueue.Clear();
        questProductList = requiredProducts;

        foreach (string sentence in dialogue.sentences)
        {
            dialogueQueue.Enqueue(sentence);
        }
        dialogueQueue.Enqueue(dialogue.questSentence);
        ActivatePlayerMovement(false);
        DisplayNextDialogue();
        dialogueInitiated = true;
    }
    #endregion

    #region Private Functions

    private void DisplayNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = dialogueQueue.Dequeue();
        textMeshPro.SetText(sentence);
        //Debug.Log($"sentence: {sentence}");
    }

    private void EndDialogue()
    {
        Debug.Log("End of dialogueQueue");
        dialogueInitiated = false;
        DisplayInstruction(false, false);
        ActivateQuestItems(questProductList);
        ActivatePlayerMovement(true);
    }

    private void ActivatePlayerMovement(bool activate)
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = activate;
        }
    }

    private void ActivateQuestItems(List<string> requiredProducts)
    {
        foreach (string product in requiredProducts)
        {
            questManager.ActivateQuestProduct(true, product);
        }

        Debug.Log("Quest Items activated");
    }
    #endregion

}