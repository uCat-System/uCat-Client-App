using System;
using System.Collections.Generic;
using UnityEngine;

public class UICommandHandler
{
    // <Summary>
    // This class is used to handle UI Command responses from the user, making them modular.
    // </Summary>

    private static Dictionary<string, MenuNavigationResponseType> menuNavigationActions;

    private static Dictionary<string, MenuActivationResponseType> menuActivationActions;


    public enum MenuActivationResponseType {
        POSITIVE_ACTIVATE_MENU_RESPONSE,
        UNKNOWN_ACTIVATION_RESPONSE
    }
    public enum MenuNavigationResponseType
    {
        NURSE_RESPONSE,

        RESTART_LEVEL_RESPONSE,

        RESUME_RESPONSE,

        RECITE_WORDS_RESPONSE,
        RECITE_SENTENCES_RESPONSE,
        RECITE_OPEN_QUESTIONS_RESPONSE,
        WRITING_RESPONSE,
        CONVERSATION_RESPONSE,
        SETTINGS_RESPONSE,
        UNKNOWN_NAVIGATION_RESPONSE
    }

    static UICommandHandler()
    {
        // Menu Navigation commands

        NavigationInputData navigationInputData = Resources.Load<NavigationInputData>("NavigationInputData");
        menuNavigationActions = new Dictionary<string, MenuNavigationResponseType>
        {
            // { navigationInputData.repeatLevelInput, MenuNavigationResponseType.REPEAT_LEVEL_RESPONSE },
            // { navigationInputData.nurseInput, MenuNavigationResponseType.NURSE_RESPONSE },
            // { navigationInputData.restartLevelInput, MenuNavigationResponseType.RESTART_LEVEL_RESPONSE },
            // { navigationInputData.resumeInput, MenuNavigationResponseType.RESUME_RESPONSE },
            // { navigationInputData.reciteWordsInput, MenuNavigationResponseType.RECITE_WORDS_RESPONSE },
            // { navigationInputData.reciteSentencesInput, MenuNavigationResponseType.RECITE_SENTENCES_RESPONSE },
            // { navigationInputData.reciteOpenQuestionsInput, MenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE },
        };

        foreach (string nurseInput in navigationInputData.nurseInputs)
        {
             menuNavigationActions[nurseInput] = MenuNavigationResponseType.NURSE_RESPONSE;
        }

        foreach (string restartLevelInput in navigationInputData.restartLevelInputs)
        {
             menuNavigationActions[restartLevelInput] = MenuNavigationResponseType.RESTART_LEVEL_RESPONSE;
        }

        foreach (string resumeInput in navigationInputData.resumeInputs)
        {
             menuNavigationActions[resumeInput] = MenuNavigationResponseType.RESUME_RESPONSE;
        }

        foreach (string reciteWordsInput in navigationInputData.reciteWordsInputs)
        {
             menuNavigationActions[reciteWordsInput] = MenuNavigationResponseType.RECITE_WORDS_RESPONSE;
        }

        foreach (string reciteSentencesInput in navigationInputData.reciteSentencesInputs)
        {
             menuNavigationActions[reciteSentencesInput] = MenuNavigationResponseType.RECITE_SENTENCES_RESPONSE;
        }

        foreach (string reciteOpenQuestionsInput in navigationInputData.reciteOpenQuestionsInputs)
        {
             menuNavigationActions[reciteOpenQuestionsInput] = MenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE;
        }

        foreach (string writingInput in navigationInputData.writingInputs)
        {
             menuNavigationActions[writingInput] = MenuNavigationResponseType.WRITING_RESPONSE;
        }

        foreach (string conversationInput in navigationInputData.conversationInputs)
        {
             menuNavigationActions[conversationInput] = MenuNavigationResponseType.CONVERSATION_RESPONSE;
        }

        foreach (string settingsInput in navigationInputData.settingsInputs)
        {
             menuNavigationActions[settingsInput] = MenuNavigationResponseType.SETTINGS_RESPONSE;
        }


        // Menu Activation Commands

        MenuActivationInputData activationInputData = Resources.Load<MenuActivationInputData>("MenuActivationInputData");
        menuActivationActions = new Dictionary<string, MenuActivationResponseType> {};

        foreach (string acceptableWakeWord in activationInputData.acceptableWakeWords)
        {
             menuActivationActions[acceptableWakeWord] = MenuActivationResponseType.POSITIVE_ACTIVATE_MENU_RESPONSE;
        }


        if (navigationInputData == null)
        {
            Debug.LogError("navigationInputData not found.");
            return;
        }
    }

    public static MenuActivationResponseType CheckIfMenuActivationCommandsWereSpoken(string text) {

        // check if any of the arguments are null
        if (text == null)
        {
            Debug.LogError("Text is null");
            return MenuActivationResponseType.UNKNOWN_ACTIVATION_RESPONSE;
        }

        string lowercaseText = text.ToLower();

        if (menuActivationActions.ContainsKey(lowercaseText))
        {
            Debug.Log("Menu activation command is contained in list: " + lowercaseText);
            return menuActivationActions[lowercaseText];
        }
        else
        {
            return MenuActivationResponseType.UNKNOWN_ACTIVATION_RESPONSE;
        }
    }
    public static MenuNavigationResponseType CheckIfMenuNavigationCommandsWereSpoken(string text)
    {
        Debug.Log("Checking if menu commands were spoken?" + text);
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
