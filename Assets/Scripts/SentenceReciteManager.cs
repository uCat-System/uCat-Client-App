using System.Collections;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Json;
using MText;

public class SentenceReciteManager : MonoBehaviour
{

    private bool isDeciding = false;
    // For tracking if the user is repeating a word currently
    // private bool isLastAttemptAtWord;

    // Current word tracking
    int currentWordIndex = 0;

    // Word lists
     string[] currentWordList;
     string[] changPaperSentenceList = new string[] { 
         "How do you like my music", "My glasses are comfortable", "What do you do", "I do not feel comfortable", "Bring my glasses here",
         "You are not right", "That is very clean", "My family is here"
    };

    //string[] changPaperSentenceList = new string[] { "How do you like my music" };

    bool changComplete;
    string[] uiControlsWordList = new string[] { "go to main menu", "I would like to repeat sentences" };
    string emergencyStopWord = "emergency stop";

    public ScoreManager _scoreManager;
    public LevelManager _levelManager;

    bool uiComplete;

    // UI elements
    public TMPro.TextMeshPro reciteText;
    private Modular3DText reciteText3D;

    [SerializeField] private Wit wit;

    void Start()
    {
        uiComplete = false;
        changComplete = false;
        // isLastAttemptAtWord = false;
        _scoreManager.SetMaxScoreBasedOnWordListCount(changPaperSentenceList.Length + uiControlsWordList.Length);

        reciteText3D = GameObject.Find("ReciteText3D").GetComponent<Modular3DText>();

        // Start with the first chang word
        currentWordList = changPaperSentenceList;

        UpdateReciteTextToCurrentWord();

    }

    void UpdateReciteTextToCurrentWord()
    {
        reciteText3D.UpdateText("Sentence: " + currentWordList[currentWordIndex]);

        //reciteText.text = "Sentence: " + currentWordList[currentWordIndex];
    }

    public void OnMicrophoneTimeOut()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    IEnumerator ChangeTimeOutText()
    {
        reciteText3D.UpdateText("Timed out! Moving on...");

       // reciteText.text = "Timed out! Moving on...";
        yield return new WaitForSeconds(2);
        GoToNextWord();
    }

    public void OnMicrophoneInactivity()
    {
        Debug.Log("Inactivity");
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    void GoToNextWord()
    {
        // If the next word does not exceed the limit
        if (currentWordIndex < currentWordList.Length - 1)
        {
            currentWordIndex++;
            Debug.Log("Increased index to " + currentWordIndex);
        }


        UpdateReciteTextToCurrentWord();

    }
    void RepeatSameWord()
    {
        UpdateReciteTextToCurrentWord();
    }

  
    public void StartSentenceCheck(string text) {
        Debug.Log("Start sentece check");
        StartCoroutine(CheckRecitedSentence(text));
    }

    public IEnumerator CheckRecitedSentence(string transcription)
    {
        Debug.Log("Checking sentence with transcription");
        Debug.Log(transcription);
        bool wordAnsweredCorrectly;

        if (isDeciding) {
            if (transcription.ToLower() == "next")
            {
                Debug.Log("Going to next level");
                _levelManager.LevelComplete();

            }
            else if (transcription.ToLower() == "repeat")
            {
                Debug.Log("Repeating this level");
                _levelManager.RepeatLevel();
            }
        }

        if (transcription.Length == 0)
        {
            // In case of misinterpretation / wit error
            yield break;
        }

        if (transcription.ToLower() == emergencyStopWord)
        {
            Debug.Log("Emergency stop");
            wit.Deactivate();
            reciteText3D.UpdateText("Emergency Stop Called");
            //reciteText.text = "Emergency Stop Called";
            yield break;

        }
        // Does their answer match the current word?
        wordAnsweredCorrectly = transcription.ToLower() == currentWordList[currentWordIndex].ToLower();

        // Change text to reflect correct / incorrect 

        reciteText3D.UpdateText(wordAnsweredCorrectly ? "Correct! " : "Incorrect.");

        yield return new WaitForSeconds(2);

        if (wordAnsweredCorrectly)
        {
            WordAnsweredCorrectly();
        }
        else
        {
            StartCoroutine(WordAnsweredIncorrectly());
        }

    }
   
    void AddScoreToScoreManager()
    {
        Debug.Log("Adding score in score manager to " + _scoreManager.Level2CurrentScore.ToString());
        _scoreManager.Level2CurrentScore = _scoreManager.Level2CurrentScore + 1;
    }

    IEnumerator WordAnsweredIncorrectly()
    {

        reciteText3D.UpdateText("Try again!");

        //reciteText.text = isLastAttemptAtWord ? "Moving on..." : "Try again...";
        yield return new WaitForSeconds(1);

        RepeatSameWord();
    }

    void MoveOnIfMoreWordsInList()
    {
        if (currentWordIndex < currentWordList.Length - 1)
        {
            Debug.Log("There are more words in this list");
            GoToNextWord();
        }

        else
        {
            Debug.Log("NO MORE words in this list");
            if (currentWordList == changPaperSentenceList) { changComplete = true; }
            if (currentWordList == uiControlsWordList) { uiComplete = true; }
            StartCoroutine(CheckWordListStatus());
        }
    }
    void WordAnsweredCorrectly()
    {
        Debug.Log("Word answered correctly function");
        AddScoreToScoreManager();

        // Reset last word check, as moving on to next word
        // isLastAttemptAtWord = false;

        MoveOnIfMoreWordsInList();
    }


    IEnumerator CheckWordListStatus()
    {
        Debug.Log("Checking word list ");
        Debug.Log("Chang complete: " + changComplete);
        Debug.Log("UI complete: " + uiComplete);

        // Either proceed to next word list, or end the game.

        if (changComplete && !uiComplete)
        {
            Debug.Log("Chang complete, moving onto UI word list.");
            currentWordList = uiControlsWordList;
            currentWordIndex = 0;
            reciteText3D.UpdateText("Great! Moving onto UI word list.");
            yield return new WaitForSeconds(2);
            UpdateReciteTextToCurrentWord();

        }
        else if (changComplete && uiComplete)
        {
            Debug.Log("Both lists complete, ending game.");
            currentWordIndex = 0;
            reciteText3D.UpdateText("Finished!");
            GameOver();
        }

        else
        {
            Debug.Log("Something went wrong in conditional for list changing.");
        }
    }

    void GameOver()
    {
        reciteText3D.UpdateText("Say 'next' to proceed.\nOr 'repeat' to repeat sentences.");
        isDeciding = true;
    }
}
