using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    public class ConfirmationHandler : MonoBehaviour
    {
        // <Summary>
        // This class is used to handle confirmation responses from the user.
        // It is used in the FreeSpeechManager class (currently).
        // </Summary>

        private Dictionary<string, Action<Modular3DText, WordReciteManager, FreeSpeechManager, string>> confirmationActions;
        private Dictionary<Enum, string> confirmationResponses;

        public float amountOfTimeToWaitBeforeRepeatingWord = 2f;

        // Reference to the ConfirmationHandlerSO asset
        public ConfirmationHandlerSO confirmationHandlerSO;

        public enum ConfirmationResponseType
        {
            POSITIVE_CONFIRMATION_RESPONSE,
            NEGATIVE_CONFIRMATION_RESPONSE,
            UNKNOWN_CONFIRMATION_RESPONSE
        }

        public ConfirmationHandler()
        {
            confirmationActions = new Dictionary<string, Action<Modular3DText, WordReciteManager, FreeSpeechManager, string>>
            {
                { "yes", HandleYesConfirmation },
                { "no", HandleNoConfirmation }
                // Add potential localisation here
            };

            confirmationResponses = new Dictionary<Enum, string> {
                { ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE, "Cool!" },
                { ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE, "Oops, let's try again." },
                { ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE, "Sorry, I didn't understand that. Please say yes or no."}
            };
        }

        public void CheckIfConfirmationWasSpoken(string text, Modular3DText partialText3D, WordReciteManager _wordReciteManager, FreeSpeechManager _freeSpeechManager, string originallyUtteredText)
        {
            // check if any of the arguments are null
            if (text == null || partialText3D == null || _wordReciteManager == null || _freeSpeechManager == null || originallyUtteredText == null)
            {
                Debug.LogError("One or more arguments are null");
                // name which ones are null 
                Debug.LogError("text: " + text);
                Debug.LogError("partialText3D: " + partialText3D);
                Debug.LogError("_wordReciteManager: " + _wordReciteManager);
                Debug.LogError("_freeSpeechManager: " + _freeSpeechManager);
                Debug.LogError("originallyUtteredText: " + originallyUtteredText);

                return;
            }
            string lowercaseText = text.ToLower();

            if (confirmationActions.ContainsKey(lowercaseText))
            {
                confirmationActions[lowercaseText].Invoke(partialText3D, _wordReciteManager, _freeSpeechManager, originallyUtteredText);
            }
            else
            {
                Debug.Log("Something else was spoken");
                HandleOtherConfirmation(lowercaseText, partialText3D, _freeSpeechManager, originallyUtteredText);
            }
        }

        // Action Handlers

        private void HandleYesConfirmation(Modular3DText partialText3D, WordReciteManager _wordReciteManager,FreeSpeechManager _freeSpeechManager, string originallyUtteredText )
        {
            Debug.LogError("yes: partialText3D: " + (partialText3D == null));

            Debug.LogError("yes: _wordReciteManager: " + (_wordReciteManager == null));

            StartCoroutine(HandleYesConfirmationCoroutine(partialText3D, _wordReciteManager));
        }

        private void HandleNoConfirmation(Modular3DText partialText3D, WordReciteManager _wordReciteManager,FreeSpeechManager _freeSpeechManager, string originallyUtteredText )
        {
            StartCoroutine(HandleNoConfirmationCoroutine(partialText3D, _wordReciteManager, _freeSpeechManager, originallyUtteredText));
        }

        private void HandleOtherConfirmation(string text, Modular3DText partialText3D,FreeSpeechManager _freeSpeechManager, string originallyUtteredText)
        {
            StartCoroutine(HandleOtherConfirmationCoroutine(partialText3D, _freeSpeechManager, originallyUtteredText));
        }

        // Coroutines for handling confirmation responses, including pauses

        private IEnumerator HandleYesConfirmationCoroutine(Modular3DText partialText3D, WordReciteManager _wordReciteManager)
        {
            Debug.Log("Yes was spoken coroutine" + _wordReciteManager.name, partialText3D.gameObject );
            // partialText3D.UpdateText(confirmationResponses[ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE]);
            yield return new WaitForSeconds(2);
            // _wordReciteManager.MoveOnIfMoreWordsInList();
        }
        private IEnumerator HandleNoConfirmationCoroutine(Modular3DText partialText3D, WordReciteManager _wordReciteManager, FreeSpeechManager _freeSpeechManager, string originallyUtteredText)
        {
            partialText3D.UpdateText(confirmationResponses[ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE]);
            yield return new WaitForSeconds(amountOfTimeToWaitBeforeRepeatingWord);
            _wordReciteManager.RepeatSameWord();
        }

        private IEnumerator HandleOtherConfirmationCoroutine(Modular3DText partialText3D, FreeSpeechManager _freeSpeechManager, string originallyUtteredText)
        {
            partialText3D.UpdateText(confirmationResponses[ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE]);
            yield return new WaitForSeconds(amountOfTimeToWaitBeforeRepeatingWord);
            _freeSpeechManager.ConfirmWhatUserSaid(originallyUtteredText);
        }


        // Expose public static method for getting confirmation responses
        public static string GetConfirmationResponse(ConfirmationResponseType responseType)
        {
            ConfirmationHandlerSO confirmationHandlerSO = FindObjectOfType<ConfirmationHandler>().confirmationHandlerSO;
            switch (responseType)
            {
                case ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE:
                    return confirmationHandlerSO.PositiveConfirmationResponse;
                case ConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE:
                    return confirmationHandlerSO.NegativeConfirmationResponse;
                case ConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE:
                    return confirmationHandlerSO.UnknownConfirmationResponse;
                default:
                    return string.Empty;
            }
        }
    }
}
