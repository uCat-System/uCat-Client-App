using System;
using System.Collections.Generic;
using UnityEngine;

public class UICommandHandler
{
    // <Summary>
    // This class is used to handle UI Command responses from the user, making them modular.
    // </Summary>

    private static Dictionary<string, MenuNavigationResponseType> menuNavigationActions;

    public enum MenuNavigationResponseType
    {
        REPEAT_LEVEL_RESPONSE,
        NEXT_LEVEL_RESPONSE,

        NURSE_RESPONSE,

        RESTART_LEVEL_RESPONSE,

        RESUME_RESPONSE,

        RECITE_WORDS_RESPONSE,
        RECITE_SENTENCES_RESPONSE,
        RECITE_OPEN_QUESTIONS_RESPONSE,
        LOBBY_RESPONSE,
        UNKNOWN_NAVIGATION_RESPONSE
    }

    static UICommandHandler()
    {
        NavigationInputData navigationInputData = Resources.Load<NavigationInputData>("NavigationInputData");
        menuNavigationActions = new Dictionary<string, MenuNavigationResponseType>
        {
            { navigationInputData.repeatLevelInput, MenuNavigationResponseType.REPEAT_LEVEL_RESPONSE },
            { navigationInputData.nextLevelInput, MenuNavigationResponseType.NEXT_LEVEL_RESPONSE },
            { navigationInputData.nurseInput, MenuNavigationResponseType.NURSE_RESPONSE },
            { navigationInputData.restartLevelInput, MenuNavigationResponseType.RESTART_LEVEL_RESPONSE },
            { navigationInputData.resumeInput, MenuNavigationResponseType.RESUME_RESPONSE },
            { navigationInputData.reciteWordsInput, MenuNavigationResponseType.RECITE_WORDS_RESPONSE },
            { navigationInputData.reciteSentencesInput, MenuNavigationResponseType.RECITE_SENTENCES_RESPONSE },
            { navigationInputData.reciteOpenQuestionsInput, MenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE },
            { navigationInputData.lobbyInput, MenuNavigationResponseType.LOBBY_RESPONSE }
        };


        if (navigationInputData == null)
        {
            Debug.LogError("navigationInputData not found.");
            return;
        }
    }

    public static MenuNavigationResponseType CheckIfMenuNavigationCommandsWereSpoken(string text)
    {
        // check if any of the arguments are null
        if (text == null)
        {
            Debug.LogError("Text is null");
            return MenuNavigationResponseType.UNKNOWN_NAVIGATION_RESPONSE;
        }

        string lowercaseText = text.ToLower();

        if (menuNavigationActions.ContainsKey(lowercaseText))
        {
            return menuNavigationActions[lowercaseText];
        }
        else
        {
            return MenuNavigationResponseType.UNKNOWN_NAVIGATION_RESPONSE;
        }
    }
}
