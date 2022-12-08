using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WordReciteManager : MonoBehaviour
{
    string currentWord = string.Empty;
    string[] wordsToRecite = new string[] { "thirsty", "tired" };
    public TMPro.TextMeshPro reciteText;
    // Start is called before the first frame update
    void Start()
    {
        currentWord = wordsToRecite[0];
        reciteText.text = "Word to recite: " + currentWord;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckRecitedWord(string[] values)
    {
        Debug.Log(values[0]);
        // proceed if there was only 1 word said
        if (values.Length > 1)
        {

        }

        if (values[0].ToLower() == currentWord)
        {
            reciteText.text = "Correct! :D ";
        } else
        {
            // handle failed word utterance here later

            reciteText.text = "Incorrect :(";
        }
        

    }
}
