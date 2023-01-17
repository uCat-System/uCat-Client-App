using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.Events;

public class WordReciteManager : MonoBehaviour
{
    private bool gameIsRunning;
    // For tracking if the user is repeating a word currently
    private bool isLastAttemptAtWord;
    string currentWord = string.Empty;
    string[] currentWordList;
    string[] changPaperWordList = new string[] { "hello","thirsty", "they", "hope", "up", "goodbye", "music", "tired", "nurse", "computer" };
    string[] uiControlsWordList = new string[] { "one", "two", "three", "proceed", "next", "repeat", "back", "pause", "menu", "help" };
    int currentWordIndex = 0;
    public TMPro.TextMeshPro reciteText;

    [SerializeField] private Wit wit;

    UnityEvent changeWordEvent = new UnityEvent();
    void Start()
    {
        isLastAttemptAtWord = false;
        // Start with the first chang word
        currentWordList = changPaperWordList;
        currentWord = currentWordList[0];

        UpdateReciteText();
        // Debug only - will eventually start through voice 
        gameIsRunning = true;

        // Activate microphone
        wit.Activate();
    }

    void UpdateReciteText()
    {
        Debug.Log("updating text, current is" +currentWord);
        reciteText.text = "Word to recite: " + currentWord;
    }

    void GoToNextWord()
    {
        Debug.Log("moving to next word");
        // If the next word does not exceed the limit
        if (currentWordIndex+1 <= currentWordList.Length-1)
        {
            currentWordIndex++;
            currentWord = currentWordList[currentWordIndex];
        }
       

        UpdateReciteText();

        // Activate Wit again
        wit.Activate();
    }
    void RepeatSameWord()
    {
        UpdateReciteText();

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

        wordAnsweredCorrectly = values[0].ToLower() == currentWord;

        // Change text to reflect correct / incorrect 

        reciteText.text = wordAnsweredCorrectly ? "Correct! :D " : "Incorrect :(";
        // --> they can try again maybe in future?
        yield return new WaitForSeconds(2);

        // TODO - track user's scores here in a manager / text export somewhere
        Debug.Log("currentindex: " + currentWordIndex);
        Debug.Log("currentWordList.length: " + currentWordList.Length);
        if (currentWordIndex >= currentWordList.Length-1)
        {
            GameOver();
            yield return null;
        }
        Debug.Log("This shouldn't print if game is over");

       
        if (wordAnsweredCorrectly)
        {
            isLastAttemptAtWord = false;
            GoToNextWord();
        } else
        {
            Debug.Log("Wrong, is last att? " + isLastAttemptAtWord);
            reciteText.text = isLastAttemptAtWord ? "Moving on..." : "Try again...";
            yield return new WaitForSeconds(1);

            // If they still have 1 chance to answer
            if (!isLastAttemptAtWord)
            {
                isLastAttemptAtWord = true;
                RepeatSameWord();
            } else
            // Move onto next one
            {
                isLastAttemptAtWord = false;
                GoToNextWord();
            }
           
        }
        
    }

    void GameOver()
    {
        reciteText.text = "Word list finished";
        gameIsRunning = false;
    }
}
