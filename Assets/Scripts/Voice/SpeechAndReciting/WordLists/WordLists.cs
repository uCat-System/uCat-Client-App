using System.Collections.Generic;
using UnityEngine;


// This creates an asset that can be used to store lists of words and sentences from the inspector.
[CreateAssetMenu(menuName = "Custom/WordLists")]
public class WordLists : ScriptableObject
{
    public List<string> introWordList;
    public List<string> level1WordList;
    public List<string> level2SentenceList;
    public List<string> level3OpenQuestionsList;
    public List<AudioClip> level3OpenQuestionsAudioList;
    public List<string> level1UiList;
    public List<string> level2UiList;
}
