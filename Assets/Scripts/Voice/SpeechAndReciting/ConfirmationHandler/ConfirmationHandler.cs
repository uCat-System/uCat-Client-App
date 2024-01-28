using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ConfirmationHandler
{
    // <Summary>
    // This class is used to handle confirmation responses from the user.
    // EG 'yes/no' or 'next/repeat'.
    // It is used in the FreeSpeechManager class (currently).
    // </Summary>

    private static Dictionary<string, ConfirmationResponseType> confirmationActions;
    private static Dictionary<string, ProceedResponseType> proceedActions;

    public static Dictionary<Enum, string> proceedResponses;

    public static Dictionary<Enum, string> confirmationResponses;
    public static float confirmationWaitTimeInSeconds = 1f;


    public static AudioClip positiveConfirmationAudio;

    public enum ConfirmationResponseType
    {
        POSITIVE_CONFIRMATION_RESPONSE,
        NEGATIVE_CONFIRMATION_RESPONSE,
        UNKNOWN_CONFIRMATION_RESPONSE
    }

        public enum ProceedResponseType
    {
        POSITIVE_PROCEED_RESPONSE,
        NEGATIVE_PROCEED_RESPONSE,
        UNKNOWN_PROCEED_RESPONSE
    }

    static ConfirmationHandler()
    {
        confirmationActions = new Dictionary<string, ConfirmationResponseType>
        {
            { "yes", ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE },
            { "no", ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE },
        };

        proceedActions = new Dictionary<string, ProceedResponseType>
        {
            { "next", ProceedResponseType.POSITIVE_PROCEED_RESPONSE },
            { "repeat", ProceedResponseType.NEGATIVE_PROCEED_RESPONSE }
        };

        // Access the ConfirmationResponseData scriptable object's fields
        ConfirmationResponseData confirmationResponseData = Resources.Load<ConfirmationResponseData>("ConfirmationResponseData");
        positiveConfirmationAudio = confirmationResponseData.positiveConfirmationAudio;

        if (confirmationResponseData == null)
        {
            Debug.LogError("ConfirmationResponseData not found.");
            return;
        }

        confirmationResponses = new Dictionary<Enum, string>
        {
            { ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE, confirmationResponseData.positiveConfirmationResponse },
            { ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE, confirmationResponseData.negativeConfirmationResponse },
            { ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE, confirmationResponseData.unknownConfirmationResponse }
        };

        proceedResponses = new Dictionary<Enum, string>
        {
            {ProceedResponseType.POSITIVE_PROCEED_RESPONSE, confirmationResponseData.positiveProceedResponse },
            {ProceedResponseType.NEGATIVE_PROCEED_RESPONSE, confirmationResponseData.negativeProceedResponse },
            {ProceedResponseType.UNKNOWN_PROCEED_RESPONSE, confirmationResponseData.unknownProceedResponse },
        };
    }

    public static ConfirmationResponseType CheckIfConfirmationWasSpoken(string text)
    {
        // check if any of the arguments are null
        if (text == null)
        {
            Debug.LogError("Text is null");
            return ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE;
        }

        string lowercaseText = text.ToLower();

        if (confirmationActions.ContainsKey(lowercaseText))
        {
            return confirmationActions[lowercaseText];
        }

        else
        {
            return ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE;
        }
    }

     public static ProceedResponseType CheckIfProceedPhraseWasSpoken(string text)
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
