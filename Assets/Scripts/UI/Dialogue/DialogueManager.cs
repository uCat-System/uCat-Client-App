using UnityEngine;
using MText;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public Modular3DText subtitleText;

    public AnimationDriver catAnimationDriver;

    public LevelManager _levelManager;

    public WordReciteManager _wordReciteManager;

    public AudioSource catAudioSource;

    public bool introDialogueIsComplete;

    void Start()
    {
        // uCat begins idle so that the first anim can play properly
        introDialogueIsComplete = false;
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        StartCoroutine(CycleThroughDialogue());
    }

   public void SetDialogueTextAnimationAndSound(Dictionary<int, string> dialogueList, 
        Dictionary<int, AnimationDriver.CatAnimations> dialogueAnimations, Dictionary<int, AudioClip> dialogueAudio)
    {
        Debug.Log("SetDialogueTextAnimationAndSound" + DialogueHandler.currentDialogueOptionIndex + " " + dialogueList.Count + " " + dialogueAnimations.Count);
        if (dialogueList.TryGetValue(DialogueHandler.currentDialogueOptionIndex, out string currentDialogueOption))
        {
            // Update dialogue
            subtitleText.UpdateText(currentDialogueOption);
            Debug.Log("Update dialogue " + currentDialogueOption);


            // Play the relevant animation
            catAnimationDriver.catAnimation = dialogueAnimations[DialogueHandler.currentDialogueOptionIndex];
            Debug.Log("Update catAnimation " + dialogueAnimations[DialogueHandler.currentDialogueOptionIndex]);

            // Play the dialogue audio
            // catAudioSource.clip = dialogueAudio[UcatDialogueHandler.currentDialogueOptionIndex];
            catAudioSource.PlayOneShot(dialogueAudio[DialogueHandler.currentDialogueOptionIndex]);
        }
        else
        {
            Debug.LogError("Invalid current dialogue option index: " + DialogueHandler.currentDialogueOptionIndex);
        }
    }

    private IEnumerator CycleThroughDialogue() {
        // TODO move this out of ienumerator, only need to do it once
        Debug.Log("Cycling through with level " + _levelManager.currentLevel);

        Dictionary<int, string> currentDialogueList;
        Dictionary<int, AnimationDriver.CatAnimations> currentAnimationList;
        Dictionary<int, AudioClip> currentAudioList;
         // We pass in different dictionaries based on the scene
        switch (_levelManager.currentLevel) {
            case "Intro":
                currentDialogueList = DialogueHandler.uCatIntroDialogue;
                currentAnimationList = DialogueHandler.uCatIntroDialogueAnimations;
                currentAudioList = DialogueHandler.uCatIntroDialogueAudio;
                break;
            case "Level1":
                Debug.Log("Level1");
                currentDialogueList = DialogueHandler.uCatLevel1Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel1DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel1DialogueAudio;
                break;
            case "Level2":
                currentDialogueList = DialogueHandler.uCatLevel2Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel2DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel2DialogueAudio;
                break;
            case "Level3":
                currentDialogueList = DialogueHandler.uCatLevel3Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel3DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel3DialogueAudio;
                break;
            default:
                currentDialogueList = null;
                currentAnimationList = null;
                currentAudioList = null;
                Debug.LogError("Dictionary not setup for: " + _levelManager.currentLevel);
                break;
        }

        SetDialogueTextAnimationAndSound(currentDialogueList, currentAnimationList, currentAudioList);
        yield return new WaitWhile(() => catAudioSource.isPlaying);
        yield return new WaitForSeconds(DialogueHandler.timeBetweenLinesInSeconds);
        
        Debug.Log("Checking if we should continue" + DialogueHandler.currentDialogueOptionIndex + " " + currentDialogueList.Count + " " + currentAnimationList.Count);
        if (DialogueHandler.currentDialogueOptionIndex >= currentDialogueList.Count-1 || currentDialogueList == null || currentAnimationList == null) {
            // Do not continue incrementing if we are at the end
            Debug.Log("END OF DIALOGUE");
            EndOfDialogue();
            yield break;
        } else {
            // Otherwise, start the next line
            DialogueHandler.IncrementDialogueOption();
            StartCoroutine(CycleThroughDialogue());
        }
    }

    void EndOfDialogue() {
        DialogueHandler.currentDialogueOptionIndex = 0;
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        switch (_levelManager.currentLevel) {
            case "Intro":
                introDialogueIsComplete = true;
                _wordReciteManager.enabled = true;
                // Some function in wordrecitemanager to test the hello thing
                break;
            case "Level1":
                // TODO: do something here
                break;
            case "Level2":
                // TODO: do something here
                break;
            case "Level3":
                // TODO: do something here
                break;
            default:
                Debug.LogError("Dictionary not setup for: " + _levelManager.currentLevel);
                break;
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCoroutine(CycleThroughDialogue(_levelManager.currentLevel));
        //     // SetDialogueTextAnimationAndSound();
        // }
    }
}
