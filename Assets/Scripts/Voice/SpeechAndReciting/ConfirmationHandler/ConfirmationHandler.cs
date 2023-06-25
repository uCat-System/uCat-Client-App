using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationHandler
{
    // <Summary>
    // This class is used to handle confirmation responses from the user.
    // It is used in the FreeSpeechManager class (currently).
    // </Summary>

    private static Dictionary<string, ConfirmationResponseType> confirmationActions;
    public static Dictionary<Enum, string> confirmationResponses;
    public static float confirmationWaitTimeInSeconds = 2f;

    public enum ConfirmationResponseType
    {
        POSITIVE_CONFIRMATION_RESPONSE,
        NEGATIVE_CONFIRMATION_RESPONSE,
        UNKNOWN_CONFIRMATION_RESPONSE
    }

    static ConfirmationHandler()
    {
        confirmationActions = new Dictionary<string, ConfirmationResponseType>
        {
            { "yes", ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE },
            { "no", ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE }
        };

         // Access the ConfirmationResponseData scriptable object's fields
        ConfirmationResponseData confirmationResponseData = Resources.Load<ConfirmationResponseData>("ConfirmationResponseData");
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
}
