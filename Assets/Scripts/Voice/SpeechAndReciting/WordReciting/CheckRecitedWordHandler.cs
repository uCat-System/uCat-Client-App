using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckRecitedWordHandler
{
    // <Summary>
    // This class is used to handle recited words from the user.
    // </Summary>


    /* 
        --> Needs to handle (isDeciding), which should check for 'next' and 'repeat' as inputs
        --> Needs to handle checking if the word was correct or not,
            so will need (text, wordToRecite, isDeciding) as inputs

    */
    public enum ProceedResponseType
    {
        POSITIVE_PROCEED_RESPONSE,
        NEGATIVE_PROCEED_RESPONSE,
        UNKNOWN_PROCEED_RESPONSE
    }

    public enum CorrectResponseType
    {
        POSITIVE_CORRECT_RESPONSE,
        NEGATIVE_CORRECT_RESPONSE,
        UNKNOWN_CORRECT_RESPONSE
    }

    // Proceeding
    private static Dictionary<string, ProceedResponseType> proceedActions;
    public static Dictionary<Enum, string> proceedResponses;

    // Checking words

    private static Dictionary<bool, CorrectResponseType> correctActions;
    public static Dictionary<Enum, string> correctResponses;

    static CheckRecitedWordHandler()
    { 
        correctActions = new Dictionary<bool, CorrectResponseType>
        {
            { true, CorrectResponseType.POSITIVE_CORRECT_RESPONSE },
            { false, CorrectResponseType.NEGATIVE_CORRECT_RESPONSE }
        };

        correctResponses = new Dictionary<Enum, string>
        {
            { CorrectResponseType.POSITIVE_CORRECT_RESPONSE, "Great job!" },
            { CorrectResponseType.NEGATIVE_CORRECT_RESPONSE, "Sorry, that's not correct." },
            { CorrectResponseType.UNKNOWN_CORRECT_RESPONSE, "Sorry, I didn't understand that." }
        };

        proceedActions = new Dictionary<string, ProceedResponseType>
        {
            { "next", ProceedResponseType.POSITIVE_PROCEED_RESPONSE },
            { "repeat", ProceedResponseType.NEGATIVE_PROCEED_RESPONSE }
        };

        proceedResponses = new Dictionary<Enum, string>
        {
            { ProceedResponseType.UNKNOWN_PROCEED_RESPONSE, "Sorry, I didn't understand that. Please say yes or no." }
        };
    }

    public static CorrectResponseType CheckIfWordOrSentenceIsCorrect(string utteredWordOrSentence, string wordToRecite)
    {
        // check if any of the arguments are null
        if (utteredWordOrSentence == null || wordToRecite == null)
        {
            Debug.LogError("Uttered word or sentence to recite is null");
            return CorrectResponseType.UNKNOWN_CORRECT_RESPONSE;
        }

        string lowercaseUtteredWordOrSentence = utteredWordOrSentence.ToLower();
        string lowercaseWordToRecite = wordToRecite.ToLower();

        if (lowercaseUtteredWordOrSentence == lowercaseWordToRecite)
        {
            return correctActions[true];
        }
        else
        {
            return correctActions[false];
        }

    }

    public static ProceedResponseType CheckIfProceedPhraseSpoken(string text)
    {
        // check if any of the arguments are null
        if (text == null)
        {
            Debug.LogError("Text is null");
            return ProceedResponseType.UNKNOWN_PROCEED_RESPONSE;
        }

        string lowercaseText = text.ToLower();

        if (proceedActions.ContainsKey(lowercaseText))
        {
            return proceedActions[lowercaseText];
        }
        else
        {
            return ProceedResponseType.UNKNOWN_PROCEED_RESPONSE;
        }
    }
}
