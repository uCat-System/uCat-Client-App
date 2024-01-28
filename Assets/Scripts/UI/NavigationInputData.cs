using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NavigationInputData", menuName = "ScriptableObjects/Navigation Input Data")]
public class NavigationInputData : ScriptableObject
{
    public List<string> nurseInputs = new List<string> {"nurse"};
    public List<string> restartLevelInputs = new List<string> {"restart level"};
    public List<string> resumeInputs = new List<string> {"resume"};
    public List<string> reciteWordsInputs = new List<string> {"recite words"};
    public List<string> reciteSentencesInputs = new List<string> {"recite sentences"};
    public List<string> reciteOpenQuestionsInputs = new List<string> {"recite open questions"};
    public List<string> writingInputs = new List<string> {"writing"};
    public List<string> conversationInputs = new List<string> {"a conversation"};
    public List<string> settingsInputs = new List<string> {"settings"}; 
}
