using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Meta.WitAi;
using MText;

public class WordReciteManager : MonoBehaviour
{
    private bool isDeciding = false;

    // For tracking if the user is repeating a word currently
    // private bool isLastAttemptAtWord;
    
    // Current word tracking
    int currentWordIndex;

    // Word lists
    string[] currentWordList;
    string[] changPaperWordList = new string[] { "hello","thirsty", "they", "hope", "up", "goodbye", "music", "tired", "nurse", "computer" };
    bool changComplete;

    // Text colours

    public Material correctColour;
    public Material incorrectColour;
    public Material defaultColour;
    public Material listeningColour;

    string[] uiControlsWordList = new string[] { "one", "two", "three", "proceed", "next", "repeat", "back", "pause", "menu", "help" };
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

        _scoreManager.Level1MaxScore = (changPaperWordList.Length + uiControlsWordList.Length); // TODO set dynamically based on level
        reciteText3D = GameObject.Find("ReciteText3D").GetComponent<Modular3DText>();
        reciteText3D.Material = defaultColour;

        // Start with the first chang word
        currentWordIndex = 0;
        currentWordList = changPaperWordList; // TODO set dynamically based on level

        StartCoroutine(StartCurrentWordCountdown());
    }

    private IEnumerator StartCurrentWordCountdown()
    {
        reciteText3D.UpdateText("..." + currentWordList[currentWordIndex] + "...");
        yield return new WaitForSeconds(1);
        reciteText3D.UpdateText(".." + currentWordList[currentWordIndex] + "..");
        yield return new WaitForSeconds(1);
        reciteText3D.UpdateText("." + currentWordList[currentWordIndex] + ".");
        yield return new WaitForSeconds(1);
        reciteText3D.UpdateText(currentWordList[currentWordIndex]);
        reciteText3D.Material = listeningColour;
    }

    public void OnMicrophoneTimeOut()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    IEnumerator ChangeTimeOutText()
    {
        //reciteText.text = "Timed out! Moving on...";
        reciteText3D.UpdateText("Timed out! Moving on...");
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
        if (currentWordIndex < currentWordList.Length-1)
        {
            currentWordIndex++;
            Debug.Log("Increased index to " + currentWordIndex);
        }

        StartCoroutine(StartCurrentWordCountdown());

    }
    void RepeatSameWord()
    {
        StartCoroutine(StartCurrentWordCountdown());
    }
    public void StartWordCheck(string transcription)
    {
        Debug.Log("recieved " + transcription);
        StartCoroutine(CheckRecitedWord(transcription));
    }
   
    public IEnumerator CheckRecitedWord(string text)
    {
        Debug.Log("Coroutine " + text);
        bool wordAnsweredCorrectly;

         if (isDeciding) {
            if (text.ToLower() == "next")
            {
                Debug.Log("Going to next level");
                _levelManager.LevelComplete();

            }
            else if (text.ToLower() == "repeat")
            {
                Debug.Log("Repeating this level");
                _levelManager.RepeatLevel();
            }
        }

        // Does their answer match the current word?
        wordAnsweredCorrectly = text.ToLower() == currentWordList[currentWordIndex].ToLower();
        Debug.Log(wordAnsweredCorrectly);
       // Debug.Log("Word answered correctly? " + wordAnsweredCorrectly);
        Debug.Log("current: " + currentWordList[currentWordIndex] + " answer: " + text.ToLower() + " " + wordAnsweredCorrectly);
        // Change text to reflect correct / incorrect 

        reciteText3D.UpdateText(wordAnsweredCorrectly ? "Correct! :D " : "Incorrect :(");

       // reciteText.text = wordAnsweredCorrectly ? "Correct! :D " : "Incorrect :(";
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
        Debug.Log("Adding score in word manager");
        _scoreManager.Level1CurrentScore = _scoreManager.Level1CurrentScore + 1;
    }

    IEnumerator WordAnsweredIncorrectly()
    {

        reciteText3D.UpdateText("Try again!");
        yield return new WaitForSeconds(1);
        RepeatSameWord();
    }

    void MoveOnIfMoreWordsInList ()
    {
        if (currentWordIndex < currentWordList.Length - 1)
        {
            Debug.Log("There are more words in this list");
            GoToNextWord();
        }

        else
        {
            Debug.Log("NO MORE words in this list");
            if (currentWordList == changPaperWordList) { changComplete = true; }
            if (currentWordList == uiControlsWordList) { uiComplete = true; }
            StartCoroutine(CheckWordListStatus());
        }
    }
    void WordAnsweredCorrectly()
    {
        AddScoreToScoreManager();
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
            reciteText3D.UpdateText("Great! Moving onto UI word list.");

            //reciteText.text = "Great! Moving onto UI word list.";
            yield return new WaitForSeconds(2);
            StartCoroutine(StartCurrentWordCountdown());
            
        }
        else if (changComplete && uiComplete)
        {
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
