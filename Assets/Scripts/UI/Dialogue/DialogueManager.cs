using UnityEngine;
using MText;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public Modular3DText subtitleText;

    public AnimationDriver catAnimationDriver;

    public LevelManager _levelManager;

    void Start()
    {
        // StartCoroutine(CycleThroughDialogue(_levelManager.currentLevel));
    }

   public void SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation(Dictionary<int, string> dialogueList, Dictionary<int, AnimationDriver.CatAnimations> dialogueAnimations)
    {
        if (dialogueList.TryGetValue(UcatDialogueHandler.currentDialogueOptionIndex, out string currentDialogueOption))
        {
            // Update dialogue
            subtitleText.UpdateText(currentDialogueOption);
             Debug.Log("Update dialogue " + currentDialogueOption);


            // Play the relevant animation
            catAnimationDriver.catAnimation = dialogueAnimations[UcatDialogueHandler.currentDialogueOptionIndex];
                         Debug.Log("Update catAnimation " + dialogueAnimations[UcatDialogueHandler.currentDialogueOptionIndex]);

        }
        else
        {
            Debug.LogError("Invalid current dialogue option index: " + UcatDialogueHandler.currentDialogueOptionIndex);
        }
    }

    private IEnumerator CycleThroughDialogue(string scene) {
        Debug.Log("Cycling through with level " + scene);
        Dictionary<int, string> currentDialogueList;
        Dictionary<int, AnimationDriver.CatAnimations> currentAnimationList;

        // We pass in different dictionaries based on the scene
        switch (scene) {
            case "Intro":
            
                currentDialogueList = UcatDialogueHandler.uCatIntroDialogue;
                currentAnimationList = UcatDialogueHandler.uCatIntroDialogueAnimations;
                break;
            default:
                currentDialogueList = null;
                currentAnimationList = null;
                Debug.LogError("Dictionary not setup for: " + scene);
                break;
        }

        SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation(UcatDialogueHandler.uCatIntroDialogue, UcatDialogueHandler.uCatIntroDialogueAnimations);
        yield return new WaitForSeconds(UcatDialogueHandler.timeBetweenLinesInSeconds);
        
        if (UcatDialogueHandler.currentDialogueOptionIndex >= currentDialogueList.Count || currentDialogueList == null || currentAnimationList == null) {
            // Do not continue incrementing if we are at the end
            yield break;
        }

        // Otherwise, start the next line
        UcatDialogueHandler.IncrementDialogueOption();
        StartCoroutine(CycleThroughDialogue(scene));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CycleThroughDialogue(_levelManager.currentLevel));
            // SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation();
        }
    }
}
