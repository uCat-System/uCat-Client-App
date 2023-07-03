using System;
using System.Collections.Generic;
using UnityEngine;

public class UcatDialogueHandler
{
    // Variable to track the current dialogue option
    public static int currentDialogueOptionIndex;

    // Dictionary to map dialogue options to their respective index
    public static Dictionary<int, string> uCatIntroDialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel1Dialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel2Dialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel3Dialogue = new Dictionary<int, string>();

    // --> {0, "Hello, I'm Ucat!"}
    // --> {1, "I'm here to help you learn about the world!"}
    public static Dictionary<int, AnimationDriver.CatAnimations> uCatIntroDialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel1DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel2DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel3DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    // --> {0, AnimationDriver.CatAnimations.Happy}
    // --> {1, AnimationDriver.CatAnimations.Sad}
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
        uCatIntroDialogueAnimations.Clear();
        uCatLevel1Dialogue.Clear();
        uCatLevel1DialogueAnimations.Clear();
        uCatLevel2Dialogue.Clear();
        uCatLevel2DialogueAnimations.Clear();
        uCatLevel3Dialogue.Clear();
        uCatLevel3DialogueAnimations.Clear();

        PopulateDictionaries(dialogueScriptData.introScriptDialogueOptions, dialogueScriptData.introScriptDialogueAnimations, uCatIntroDialogue, uCatIntroDialogueAnimations);
        PopulateDictionaries(dialogueScriptData.level1ScriptDialogueOptions, dialogueScriptData.level1ScriptDialogueAnimations, uCatLevel1Dialogue, uCatLevel1DialogueAnimations);
        PopulateDictionaries(dialogueScriptData.level2ScriptDialogueOptions, dialogueScriptData.level2ScriptDialogueAnimations, uCatLevel2Dialogue, uCatLevel2DialogueAnimations);
        PopulateDictionaries(dialogueScriptData.level3ScriptDialogueOptions, dialogueScriptData.level3ScriptDialogueAnimations, uCatLevel3Dialogue, uCatLevel3DialogueAnimations);

    }

    public static void PopulateDictionaries(List<string> dialogueOptions, List<AnimationDriver.CatAnimations> animationOptions, Dictionary<int, string> dialogueDict, Dictionary<int, AnimationDriver.CatAnimations> animationDict) {
         // // Populate the dictionary with the dialogue options
         Debug.Log("populating" + dialogueOptions.Count);
        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            string dialogueOption = dialogueOptions[i];

            // Add the dialogue option to the dictionary
            dialogueDict[i] = dialogueOption;
        }

        // Populate the second dictionary with the animations
        for (int i = 0; i < animationOptions.Count; i++)
        {
            AnimationDriver.CatAnimations animation = animationOptions[i];

            // Add the animation to the dictionary
            animationDict[i] = animation;
        }
    }


    public static void IncrementDialogueOption()
    {
        // Increment the index (will loop back to the start eventually)
        currentDialogueOptionIndex = (currentDialogueOptionIndex + 1) % uCatIntroDialogue.Count;
    }
}
