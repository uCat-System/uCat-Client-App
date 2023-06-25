using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "RecitedWordData", menuName = "ScriptableObjects/Recited Word Data")]
public class RecitedWordData : ScriptableObject
{
    public string positiveCorrectResponse = "Correct!";
    public string negativeCorrectResponse = "Incorrect.";
    public string unknownCorrectResponse = "Sorry, I didn't understand that.";   
}
