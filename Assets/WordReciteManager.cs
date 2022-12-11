using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.Events;

public class WordReciteManager : MonoBehaviour
{
    private bool gameIsRunning;
    string currentWord = string.Empty;
    int currentWordIndex = 0;
    string[] wordsToRecite = new string[] { "thirsty", "I", "tired", "are", "up" };
    public TMPro.TextMeshPro reciteText;

    [SerializeField] private Wit wit;

    UnityEvent changeWordEvent = new UnityEvent();
    void Start()
    {
        currentWord = wordsToRecite[0];

        UpdateReciteText();
        // Debug - will eventually start through voice 
        gameIsRunning = true;

        wit.Activate();
    }

    void UpdateReciteText()
    {
        reciteText.text = "Word to recite: " + currentWord;
    }

    void GoToNextWord()
    {
       
        currentWordIndex++;
        currentWord = wordsToRecite[currentWordIndex];

        UpdateReciteText();

        // Activate Wit again
        wit.Activate();
    }

    public void StartWordCheck(string[] values)
    {
        Debug.Log("word check, " + values[0]);
        // This function is called from wit (callback).
        // Launches CheckRecitedWord so that we can use IEnumerators
        StartCoroutine(CheckRecitedWord(values));
    }
    public IEnumerator CheckRecitedWord(string[] values)
    {

        Debug.Log("Checked recited, " + values[0]);

        if (values.Length > 1)
        {
            // In case of misinterpretation / wit error
           yield return null;
        }

        Debug.Log("made it to 63");

        reciteText.text = values[0].ToLower() == currentWord ? "Correct! :D " : "Incorrect :(";
        yield return new WaitForSeconds(2);
        UpdateReciteText();

        // TODO - track user's scores here in a manager / text export somewhere

        if (currentWordIndex >= wordsToRecite.Length-1)
        {
            GameOver();
            yield return null;
        } else
        {
            GoToNextWord();
        }

        
    }

    void GameOver()
    {
        reciteText.text = "Word list finished";
        gameIsRunning = false;
    }
}
