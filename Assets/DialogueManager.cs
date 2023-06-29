using UnityEngine;
using MText;

public class DialogueManager : MonoBehaviour
{
    public Modular3DText subtitleText;

    void Start()
    {
        SetSubtitlesToCurrentLineOfDialogue();
    }

   public void SetSubtitlesToCurrentLineOfDialogue()
{
    if (UcatDialogueHandler.uCatIntroDialogue.TryGetValue(UcatDialogueHandler.currentDialogueOptionIndex, out string currentDialogueOption))
    {
        subtitleText.UpdateText(currentDialogueOption);
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
            SetSubtitlesToCurrentLineOfDialogue();
        }
    }
}
