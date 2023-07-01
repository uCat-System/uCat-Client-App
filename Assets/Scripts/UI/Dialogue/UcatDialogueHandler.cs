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

    public static int timeBetweenLinesInSeconds = 0;

    static UcatDialogueHandler()
    {
        currentDialogueOptionIndex = 0;

        DialogueScriptData dialogueScriptData = Resources.Load<DialogueScriptData>("DialogueScriptData");

        if (dialogueScriptData == null)
        {
            Debug.LogError("dialogueScriptData not found.");
            return;
        }

        // The delay in seconds from data object
        timeBetweenLinesInSeconds = dialogueScriptData.timeBetweenLinesInSeconds;

        // Clear the existing dictionary
        uCatIntroDialogue.Clear();

        // Populate the dictionary with the dialogue options
        for (int i = 0; i < dialogueScriptData.introScriptDialogueOptions.Count; i++)
        {
            string dialogueOption = dialogueScriptData.introScriptDialogueOptions[i];

            // Add the dialogue option to the dictionary
            uCatIntroDialogue[i] = dialogueOption;
        }

        // Populate the second dictionary with the animations
        for (int i = 0; i < dialogueScriptData.introScriptDialogueAnimations.Count; i++)
        {
            AnimationDriver.CatAnimations animation = dialogueScriptData.introScriptDialogueAnimations[i];

            // Add the animation to the dictionary
            uCatIntroDialogueAnimations[i] = animation;
        }
    }

    public static void IncrementDialogueOption()
    {
        // Increment the index (will loop back to the start eventually)
        currentDialogueOptionIndex = (currentDialogueOptionIndex + 1) % uCatIntroDialogue.Count;
    }
}
