using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "DialogueScriptData", menuName = "ScriptableObjects/Dialogue Script Data")]
public class DialogueScriptData : ScriptableObject
{

    // Scripts & Animations
    public List<string> introScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> introScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();

    public List<string> level1ScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> level1ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();

    public List<string> level2ScriptDialogueOptions = new List<string>();
    public List<AnimationDriver.CatAnimations> level2ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();

    public List<string> level3ScriptDialogueOptions = new List<string>();

    public List<AnimationDriver.CatAnimations> level3ScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();





    public int timeBetweenLinesInSeconds = 0;

}
