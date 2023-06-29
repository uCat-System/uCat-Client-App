// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MText;

// public class UcatDialogueManager : MonoBehaviour
// {
//     // <Summary>
//     // This script is used to manage the dialogue of the Ucat character,
//     // including animations, waiting between lines, and the text that is displayed.
//     // </Summary>

//     /*

//         We should have an external script handler that handles each line of dialogue as a separate string
//         and then passes it to this script to be displayed. This script should also handle the animations
//         of the character, and the timing between lines.

//     */

//     // public AnimationDriver uCatAnimationDriver;
//     // public Modular3DText subtitleText;

//     // public float timeBetweenLines = 2f;

//     // the dictionary should be in the format of:
//     // "key" : "value"
//     // where key is the name of the dialogue line, and value is the actual dialogue line.
//     // EG "INTRO_LINE_01" : "Hello, I am Ucat. I am here to help you learn about your BCI."

//     public enum UcatIntroDialogueLines {
//         // There should be a variable amount of lines
//         // for the intro dialogue, so we should use an enum
//         // to define the lines.
//         INTRO_LINE_01,
//         INTRO_LINE_02,
//         INTRO_LINE_03,
//         INTRO_LINE_04,
//         INTRO_LINE_05,
//     }
//     // DialogueScriptEditor dialogueScriptEditor = Resources.Load<DialogueScriptEditor>("DialogueScriptEditor");

//     // This defines the dialogue for the intro.
//     // public Dictionary<UcatIntroDialogueLines, string> uCatIntroDialogue = new Dictionary<UcatIntroDialogueLines, string>() {
//         // {UcatIntroDialogueLines.INTRO_LINE_01, dialogueScriptEditor.introScriptDialogueOptions[0]},
//         // {UcatIntroDialogueLines.INTRO_LINE_02, "I'm uCat, your AI guide for your BCI. I'll take you through the calibration program."},
//     // };

//     // We need another dictionary to define which animation to play for each line of dialogue.
//     public Dictionary<UcatIntroDialogueLines, AnimationDriver.CatAnimations> uCatIntroDialogueAnimations = new Dictionary<UcatIntroDialogueLines, AnimationDriver.CatAnimations>() {
//         {UcatIntroDialogueLines.INTRO_LINE_01, AnimationDriver.CatAnimations.Surprised},
//         {UcatIntroDialogueLines.INTRO_LINE_02, AnimationDriver.CatAnimations.Happy},
//     };

//     public UcatIntroDialogueLines currentLineOfDialogue = UcatIntroDialogueLines.INTRO_LINE_01;


//     void DisplayDialogueLineAndAnimateAccordingly()
//     {
//         // To test, let's just display the first line of dialogue.
//         // and play the according animation
//         subtitleText.UpdateText(uCatIntroDialogue[currentLineOfDialogue]);
//         uCatAnimationDriver.catAnimation = uCatIntroDialogueAnimations[currentLineOfDialogue];
//         // increase currentLineOfDialogue to the next line of dialogue in the enum list
//         // and then wait for the timeBetweenLines before displaying the next line.
//         currentLineOfDialogue++;
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             DisplayDialogueLineAndAnimateAccordingly();
//         }
//     }
// }
