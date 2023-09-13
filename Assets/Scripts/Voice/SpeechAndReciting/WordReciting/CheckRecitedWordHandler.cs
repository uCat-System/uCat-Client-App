using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckRecitedWordHandler
{
    // <Summary>
    // This class is used to handle recited words from the user.
    // </Summary>

    public enum CorrectResponseType
    {
        POSITIVE_CORRECT_RESPONSE,
        NEGATIVE_CORRECT_RESPONSE,
        UNKNOWN_CORRECT_RESPONSE
    }

    public static float timeBetweenWordsInSeconds = 2f;

    // Checking words

    private static Dictionary<bool, CorrectResponseType> correctActions;
    public static Dictionary<Enum, string> correctResponses;

    public static string[] negativeCorrectResponses;


    static CheckRecitedWordHandler()
    { 
        // Access the ConfirmationResponseData scriptable object's fields
        RecitedWordData recitedWordData = Resources.Load<RecitedWordData>("RecitedWordData");

        if (recitedWordData == null)
        {
            Debug.LogError("recitedWordData not found.");
            return;
        }
        
        correctActions = new Dictionary<bool, CorrectResponseType>
        {
            { true, CorrectResponseType.POSITIVE_CORRECT_RESPONSE },
            { false, CorrectResponseType.NEGATIVE_CORRECT_RESPONSE }
        };

        correctResponses = new Dictionary<Enum, string>
        {
            { CorrectResponseType.POSITIVE_CORRECT_RESPONSE, recitedWordData.positiveCorrectResponse },
            { CorrectResponseType.NEGATIVE_CORRECT_RESPONSE, recitedWordData.negativeCorrectResponses[0] },
            { CorrectResponseType.UNKNOWN_CORRECT_RESPONSE, recitedWordData.unknownCorrectResponse }
        };

        // Load the array of negative response strings on init
        negativeCorrectResponses = recitedWordData.negativeCorrectResponses;
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
}
