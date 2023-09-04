using System;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueHandler
{
    // Variable to track the current dialogue option
    public static int currentDialogueOptionIndex;

    // Dictionary to map dialogue options to their respective index
    public static Dictionary<int, string> uCatIntroDialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel1Dialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel2Dialogue = new Dictionary<int, string>();

    public static Dictionary<int, string> uCatLevel3Dialogue = new Dictionary<int, string>();

    // EG:
    // --> {0, "Hello, I'm Ucat!"}
    // --> {1, "I'm here to help you learn about the world!"}
    public static Dictionary<int, AnimationDriver.CatAnimations> uCatIntroDialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel1DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel2DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    public static Dictionary<int, AnimationDriver.CatAnimations> uCatLevel3DialogueAnimations = new Dictionary<int, AnimationDriver.CatAnimations>();

    //EG: 
    // --> {0, AnimationDriver.CatAnimations.Happy}
    // --> {1, AnimationDriver.CatAnimations.Sad}

    public static Dictionary<int, AudioClip> uCatIntroDialogueAudio = new Dictionary<int, AudioClip>();

    public static Dictionary<int, AudioClip> uCatLevel1DialogueAudio = new Dictionary<int, AudioClip>();

    public static Dictionary<int, AudioClip> uCatLevel2DialogueAudio = new Dictionary<int, AudioClip>();

    public static Dictionary<int, AudioClip> uCatLevel3DialogueAudio = new Dictionary<int, AudioClip>();

    // EG:
    // --> {0, "hello_im_ucat.mp3}
    // --> {1, "im_here_to_help_you_learn_about_the_world.mp3}

    public static int timeBetweenLinesInSeconds = 0;

    static DialogueHandler()
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
        uCatIntroDialogueAudio.Clear();

        uCatLevel1Dialogue.Clear();
        uCatLevel1DialogueAnimations.Clear();
        uCatLevel1DialogueAudio.Clear();

        uCatLevel2Dialogue.Clear();
        uCatLevel2DialogueAnimations.Clear();
        uCatLevel2DialogueAudio.Clear();

        uCatLevel3Dialogue.Clear();
        uCatLevel3DialogueAnimations.Clear();
        uCatLevel3DialogueAudio.Clear();

        PopulateDictionaries(dialogueScriptData.introScriptDialogueOptions, dialogueScriptData.introScriptDialogueAnimations, 
            dialogueScriptData.introScriptDialogueAudio, uCatIntroDialogue, uCatIntroDialogueAnimations, uCatIntroDialogueAudio);
        PopulateDictionaries(dialogueScriptData.level1ScriptDialogueOptions, dialogueScriptData.level1ScriptDialogueAnimations,
            dialogueScriptData.level1ScriptDialogueAudio, uCatLevel1Dialogue, uCatLevel1DialogueAnimations, uCatLevel1DialogueAudio);
        PopulateDictionaries(dialogueScriptData.level2ScriptDialogueOptions, dialogueScriptData.level2ScriptDialogueAnimations,
            dialogueScriptData.level2ScriptDialogueAudio, uCatLevel2Dialogue, uCatLevel2DialogueAnimations, uCatLevel2DialogueAudio);
        PopulateDictionaries(dialogueScriptData.level3ScriptDialogueOptions, dialogueScriptData.level3ScriptDialogueAnimations,
            dialogueScriptData.level3ScriptDialogueAudio, uCatLevel3Dialogue, uCatLevel3DialogueAnimations,
            uCatLevel3DialogueAudio);

    }

    public static void PopulateDictionaries(List<string> dialogueOptions, List<AnimationDriver.CatAnimations> 
        animationOptions, List<AudioClip> dialogueAudio, Dictionary<int, string> dialogueDict, Dictionary<int, AnimationDriver.CatAnimations> animationDict, Dictionary<int, AudioClip> audioDict) {
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

        // Populate the third dictionary with the audio
        for (int i = 0; i < dialogueAudio.Count; i++)
        {
            AudioClip dialogueAudioClip = dialogueAudio[i];

            // Add the dialogue option to the dictionary
            audioDict[i] = dialogueAudioClip;
        }
    }


    public static void IncrementDialogueOption()
    {
        // Increment the index (will loop back to the start eventually)
        // TODO change
        currentDialogueOptionIndex = (currentDialogueOptionIndex + 1);
    }
}
