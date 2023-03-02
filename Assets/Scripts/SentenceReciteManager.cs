using System.Collections;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Json;

public class SentenceReciteManager : MonoBehaviour
{

    private bool isDeciding = false;
    // For tracking if the user is repeating a word currently
    private bool isLastAttemptAtWord;

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


    [SerializeField] private Wit wit;

    void Start()
    {
        uiComplete = false;
        changComplete = false;
        isLastAttemptAtWord = false;

        // Start with the first chang word
        currentWordList = changPaperSentenceList;

        UpdateReciteTextToCurrentWord();

    }

    void UpdateReciteTextToCurrentWord()
    {
        reciteText.text = "Sentence: " + currentWordList[currentWordIndex];
    }

    public void OnMicrophoneTimeOut()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    IEnumerator ChangeTimeOutText()
    {
        reciteText.text = "Timed out! Moving on...";
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

   

    // This MatchIntent function is called from wit (callback).
    // It is not registered in the GUI, but is called from wit.
    [MatchIntent("recite_sentence")] 
    public void StartSentenceCheck(WitResponseNode response) {
        var transcription = response.GetTranscription();
        StartCoroutine(CheckRecitedSentence(transcription));
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
            reciteText.text = "Emergency Stop Called";
            yield break;

        }
        // Does their answer match the current word?
        wordAnsweredCorrectly = transcription.ToLower() == currentWordList[currentWordIndex].ToLower();

        // Change text to reflect correct / incorrect 

        reciteText.text = wordAnsweredCorrectly ? "Correct! :D " : "Incorrect :(";
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
        _scoreManager.Level1CurrentScore = _scoreManager.Level1CurrentScore + 1;
    }

    IEnumerator WordAnsweredIncorrectly()
    {

        reciteText.text = isLastAttemptAtWord ? "Moving on..." : "Try again...";
        yield return new WaitForSeconds(1);

        // If they still have 1 chance to answer
        if (!isLastAttemptAtWord)
        {
            isLastAttemptAtWord = true;
            RepeatSameWord();
        }
        else

        // Move onto next one
        {
            isLastAttemptAtWord = false;
            MoveOnIfMoreWordsInList();
        }
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
        AddScoreToScoreManager();

        // Reset last word check, as moving on to next word
        isLastAttemptAtWord = false;

        MoveOnIfMoreWordsInList();
    }


    IEnumerator CheckWordListStatus()
    {
        Debug.Log("Checking word list ");

        // Either proceed to next word list, or end the game.

        if (changComplete && !uiComplete)
        {
            currentWordList = uiControlsWordList;
            currentWordIndex = 0;
            reciteText.text = "Great! Moving onto UI word list.";
            yield return new WaitForSeconds(2);
            UpdateReciteTextToCurrentWord();

        }
        else if (changComplete && uiComplete)
        {
            currentWordIndex = 0;
            reciteText.text = "Finished!";
            GameOver();
        }

        else
        {
            Debug.Log("Something went wrong in conditional for list changing.");
        }
    }

    void GameOver()
    {
        reciteText.text = "Say 'next' to proceed.\nOr 'repeat' to repeat sentences.";
        isDeciding = true;
    }
}
