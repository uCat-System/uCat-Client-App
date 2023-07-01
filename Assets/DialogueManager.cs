using UnityEngine;
using MText;

public class DialogueManager : MonoBehaviour
{
    public Modular3DText subtitleText;

    public AnimationDriver catAnimationDriver;

    void Start()
    {
        SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation();
    }

   public void SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation()
{
    if (UcatDialogueHandler.uCatIntroDialogue.TryGetValue(UcatDialogueHandler.currentDialogueOptionIndex, out string currentDialogueOption))
    {
        // Update dialogue
        subtitleText.UpdateText(currentDialogueOption);

        // Play the relevant animation
        catAnimationDriver.catAnimation = UcatDialogueHandler.uCatIntroDialogueAnimations[UcatDialogueHandler.currentDialogueOptionIndex];
    }
    else
    {
        Debug.LogError("Invalid current dialogue option index: " + UcatDialogueHandler.currentDialogueOptionIndex);
    }
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UcatDialogueHandler.IncrementDialogueOption();
            SetSubtitlesToCurrentLineOfDialogueAndPlayRelevantAnimation();
        }
    }
}
