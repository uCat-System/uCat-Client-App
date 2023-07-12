using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NavigationInputData", menuName = "ScriptableObjects/Navigation Input Data")]
public class NavigationInputData : ScriptableObject
{
    public List<string> repeatLevelInputs = new List<string> {"repeat level"};
    public List<string> nurseInputs = new List<string> {"nurse"};
    public List<string> restartLevelInputs = new List<string> {"restart level"};
    public List<string> resumeInputs = new List<string> {"resume"};
    public List<string> reciteWordsInputs = new List<string> {"recite words"};
    public List<string> reciteSentencesInputs = new List<string> {"recite sentences"};
    public List<string> reciteOpenQuestionsInputs = new List<string> {"recite open questions"};
    public List<string> lobbyInputs = new List<string> {"lobby"};
}
