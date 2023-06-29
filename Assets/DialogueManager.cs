using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EIntroDialogueLines = UcatDialogueHandler.UcatIntroDialogueLines;
using MText;


public class DialogueManager : MonoBehaviour
{
    public Modular3DText subtitleText;
    void Start()
    {
        SetSubtitlesToCurrentLineOfDialogue();
    }

    void SetSubtitlesToCurrentLineOfDialogue() {
        // First just use the first line
        // then setup current line tracking in the static class
        subtitleText.UpdateText(UcatDialogueHandler.uCatIntroDialogue[EIntroDialogueLines.INTRO_LINE_02]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
