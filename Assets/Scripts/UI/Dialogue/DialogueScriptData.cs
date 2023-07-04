using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "DialogueScriptData", menuName = "ScriptableObjects/Dialogue Script Data")]
public class DialogueScriptData : ScriptableObject
{
    // Scripts, animations and sound

    // Intro
    public List<string> introScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> introScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();

    // Level 1
    public List<AudioClip> introScriptDialogueAudio = new List<AudioClip>();
    public List<string> level1ScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> level1ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();
    public List<AudioClip> level1ScriptDialogueAudio = new List<AudioClip>();



    // Level 2

    public List<string> level2ScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> level2ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();
    public List<AudioClip> level2ScriptDialogueAudio = new List<AudioClip>();


    // Level 3

    public List<string> level3ScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> level3ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();
    public List<AudioClip> level3ScriptDialogueAudio = new List<AudioClip>();

    public int timeBetweenLinesInSeconds = 0;
}
