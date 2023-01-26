using System.Collections;
using UnityEngine;
using Meta.WitAi;

public class WordReciteManager : MonoBehaviour
{
    // For tracking if the user is repeating a word currently
    private bool isLastAttemptAtWord;

    // Current word tracking
    int currentWordIndex = 0;

    // Word lists
    string[] currentWordList;
    string[] changPaperWordList = new string[] { "hello","thirsty", "they", "hope", "up", "goodbye", "music", "tired", "nurse", "computer" };
    bool changComplete;
    string[] uiControlsWordList = new string[] { "one", "two", "three", "proceed", "next", "repeat", "back", "pause", "menu", "help" };

    public ScoreManager _scoreManager;

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
        currentWordList = changPaperWordList;


        UpdateReciteTextToCurrentWord();

        // Activate microphone
        wit.Activate();
    }

    void UpdateReciteTextToCurrentWord()
    {
        reciteText.text = "Word to recite: " + currentWordList[currentWordIndex];
    }

    public void OnMicrophoneTimeOut()
    {
        Debug.Log("timed out due to time reached");
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
        Debug.Log("timed out due to inactivity");
    }

    void GoToNextWord()
    {
        // If the next word does not exceed the limit
        if (currentWordIndex+1 <= currentWordList.Length-1)
        {
            currentWordIndex++;
            Debug.Log("Increased index to " + currentWordIndex);
        }
       

        UpdateReciteTextToCurrentWord();

        // Activate Wit again
        wit.Activate();
    }
    void RepeatSameWord()
    {
        UpdateReciteTextToCurrentWord();

        // Activate Wit again
        wit.Activate();
    }

    public void StartWordCheck(string[] values)
    {
        Debug.Log("word check, " + values[0]);
        // "hello this is a phrase" --> entity 
        // hello -> api -> unity -> 3D text (hello) 
        //
        // This function is called from wit (callback).
        // Launches CheckRecitedWord so that we can use IEnumerators for pausing 

        StartCoroutine(CheckRecitedWord(values));
    }
    public IEnumerator CheckRecitedWord(string[] values)
    {
        bool wordAnsweredCorrectly;
        Debug.Log("Checked recited, " + values[0]);

        if (values.Length > 1)
        {
            Debug.Log("quit because values.length <1");
            // In case of misinterpretation / wit error
           yield return null;
        }

        // does their answer match the current word?
        wordAnsweredCorrectly = values[0].ToLower() == currentWordList[currentWordIndex];

        // Change text to reflect correct / incorrect 

        reciteText.text = wordAnsweredCorrectly ? "Correct! :D " : "Incorrect :(";
        yield return new WaitForSeconds(2);

        // TODO - track user's scores here in a manager / text export somewhere
        Debug.Log("currentindex: " + currentWordIndex);
        Debug.Log("currentWordList.length: " + currentWordList.Length);

        
        if (currentWordIndex >= currentWordList.Length - 1)
        {
            Debug.Log("Got to inside conditional.");
            if (currentWordList == changPaperWordList) { changComplete = true; }
            if (currentWordList == uiControlsWordList) { uiComplete = true; }
            StartCoroutine(CheckWordListStatus());
            yield return null;
        }
        else
        {
            StartCoroutine(HandleWordCorrectOrIncorrect(wordAnsweredCorrectly));
        }
    
    }
    void AddScoreToScoreManager()
    {
        _scoreManager.Level1CurrentScore = _scoreManager.Level1CurrentScore + 1;
    }
    IEnumerator HandleWordCorrectOrIncorrect(bool wordAnsweredCorrectly)
    {
        if (wordAnsweredCorrectly)
        {
            AddScoreToScoreManager();

            isLastAttemptAtWord = false;
            GoToNextWord();
        }
        else
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
                GoToNextWord();
            }

        }
    }

    IEnumerator CheckWordListStatus()
    {

        // Either proceed to next word list, or end the game.

        if (changComplete && !uiComplete)
        { 
            currentWordList = uiControlsWordList;
            currentWordIndex = 0;
            reciteText.text = "Great! Moving onto UI word list.";
            yield return new WaitForSeconds(2);
            UpdateReciteTextToCurrentWord();
            wit.Activate();
            
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
        reciteText.text = "Word list finished";
    }
}
