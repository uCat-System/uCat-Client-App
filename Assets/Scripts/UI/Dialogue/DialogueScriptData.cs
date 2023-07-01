using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "DialogueScriptData", menuName = "ScriptableObjects/Dialogue Script Data")]
public class DialogueScriptData : ScriptableObject
{
    public List<string> introScriptDialogueOptions = new List<string>();

    public List<AnimationDriver.CatAnimations> introScriptDialogueAnimations = new List<AnimationDriver.CatAnimations>();
}
