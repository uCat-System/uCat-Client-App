using System;
using System.Collections.Generic;
using UnityEngine;

public class UcatDialogueHandler
{
    // Variable to track the current dialogue option
    public static int currentDialogueOptionIndex;

    // Dictionary to map dialogue options to their respective index
    public static Dictionary<int, string> uCatIntroDialogue = new Dictionary<int, string>();
    public static Dictionary<int, AnimationDriver.CatAnimations> uCatIntroDialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    static UcatDialogueHandler()
    {
        currentDialogueOptionIndex = 0;

        DialogueScriptData dialogueScriptData = Resources.Load<DialogueScriptData>("DialogueScriptData");

        if (dialogueScriptData == null)
        {
            Debug.LogError("dialogueScriptData not found.");
            return;
        }

        // Clear the existing dictionary
        uCatIntroDialogue.Clear();

        // Populate the dictionary with the dialogue options
        for (int i = 0; i < dialogueScriptData.introScriptDialogueOptions.Count; i++)
        {
            string dialogueOption = dialogueScriptData.introScriptDialogueOptions[i];

            // Add the dialogue option to the dictionary
            uCatIntroDialogue[i] = dialogueOption;
            Debug.Log("After one iteration: " + uCatIntroDialogue[i]);
        }

        Debug.Log("After all: " + uCatIntroDialogue.Count);
    }

    public static void IncrementDialogueOption()
    {
        // Increment the index
        currentDialogueOptionIndex = (currentDialogueOptionIndex + 1) % uCatIntroDialogue.Count;
    }
}
