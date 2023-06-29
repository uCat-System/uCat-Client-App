using System;
using System.Collections.Generic;
using UnityEngine;

public class UcatDialogueHandler
{
    // <Summary>
    // This script is used to manage the dialogue of the Ucat character,
    // including animations, waiting between lines, and the text that is displayed.
    // </Summary>

    public enum UcatIntroDialogueLines {
        INTRO_LINE_01,
        INTRO_LINE_02,
        INTRO_LINE_03,
        INTRO_LINE_04,
        INTRO_LINE_05,
    }

    public static Dictionary<UcatIntroDialogueLines, string> uCatIntroDialogue = new Dictionary<UcatIntroDialogueLines, string>();
    public static Dictionary<UcatIntroDialogueLines, AnimationDriver.CatAnimations> uCatIntroDialogueAnimations = new Dictionary<UcatIntroDialogueLines, AnimationDriver.CatAnimations>();

    static UcatDialogueHandler
()
    {
        DialogueScriptData dialogueScriptData = Resources.Load<DialogueScriptData>("DialogueScriptData");

        if (dialogueScriptData == null)
        {
            Debug.LogError("dialogueScriptData not found.");
            return;
        }

        // Clear the existing dictionary
        uCatIntroDialogue.Clear();

        // Populate the enum with the dialogue options (variable length)
        for (int i = 0; i < dialogueScriptData.introScriptDialogueOptions.Count; i++)
        {
            UcatIntroDialogueLines enumValue = (UcatIntroDialogueLines)i;
            string dialogueOption = dialogueScriptData.introScriptDialogueOptions[i];

            // Add the dialogue option to the dictionary
            uCatIntroDialogue[enumValue] = dialogueOption;
            Debug.Log("After one iteration: " + uCatIntroDialogue[enumValue]);
        }

        Debug.Log("After all " + uCatIntroDialogue.Count);

    }
}
