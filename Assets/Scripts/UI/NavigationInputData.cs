using UnityEngine;

[CreateAssetMenu(fileName = "NavigationInputData", menuName = "ScriptableObjects/Navigation Input Data")]
public class NavigationInputData : ScriptableObject
{
    public string repeatLevelInput = "repeat level";
    public string nextLevelInput = "next level";
    public string nurseInput = "nurse";
    public string restartLevelInput = "restart level";
    public string resumeInput = "resume";
    public string reciteWordsInput = "recite words";
    public string reciteSentencesInput = "recite sentences";
    public string reciteOpenQuestionsInput = "recite open questions";
    public string lobbyInput = "lobby";
}
