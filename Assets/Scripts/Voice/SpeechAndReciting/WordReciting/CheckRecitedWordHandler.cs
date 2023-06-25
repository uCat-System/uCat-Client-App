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

    private static Dictionary<string, ProceedResponseType> proceedActions;
    public static Dictionary<Enum, string> proceedResponses;
    
    static CheckRecitedWordHandler()
    {        
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
