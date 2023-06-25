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

    // Reference to the ConfirmationResponseData scriptable object
    public static ConfirmationResponseData confirmationResponseData;

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

        confirmationResponses = new Dictionary<Enum, string>
        {
            { ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE, "Cool!" },
            { ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE, "Oops, let's try again." },
            { ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE, "Sorry, I didn't understand that. Please say yes or no." }
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
