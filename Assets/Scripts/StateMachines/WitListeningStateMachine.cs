using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/Wit Listening State Machine")]
public class WitListeningStateMachine : ScriptableObject
{
    public enum State
    {
        NotListening,
        ListeningForEverything,
        ListeningForMenuActivationCommandsOnly,
        ListeningForRecitedWordsOnly,
        ListeningForTaskMenuCommandsOnly,
        ListeningForConfirmation,

        ListeningForLobbyMenuCommandsOnly,

    }

    public enum Transition
    {
        TransitionNotListeningToMenuCommandsOnly,
        TransitionMenuCommandsOnlyToListeningForEverything,
        TransitionListeningForEverythingToListeningForRecitedWordsOnly,
        TransitionListeningForRecitedWordsOnlyToNotListening,
        TransitionListeningForRecitedWordsOnlyToListeningForTaskMenuCommandsOnly,
    }

    public State currentState;
    public List<string> menuCommandPhrases; // List of phrases for MenuCommandsOnly state
}
