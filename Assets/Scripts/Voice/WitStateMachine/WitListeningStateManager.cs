using System;
using UnityEngine;
using MText;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class WitListeningStateManager : MonoBehaviour
{
    public enum ListeningState
    {
        NotListening,
        ListeningForEverything,
        ListeningForMenuActivationCommandsOnly, // brings up menu
        ListeningForRecitedWordsOnly,
        ListeningForTaskMenuCommandsOnly, // navigates within the menu
        ListeningForConfirmation,

        ListeningForLobbyMenuCommandsOnly,
    }
    public Modular3DText listeningText3D;

    public string scene;
    public UIManager _uiManager;
    public ListeningState currentListeningState;
    public WordReciteManager _wordReciteManager;
    public GameObject[] wits;

    // This dict will return true if we are in any of the allowed reciting states

    private static Dictionary<ListeningState, bool> validRecitingStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true},
        { ListeningState.ListeningForRecitedWordsOnly, true },
        { ListeningState.ListeningForConfirmation, true },
    };

    private static Dictionary<ListeningState, bool> validMenuNavigationStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true},
        { ListeningState.ListeningForLobbyMenuCommandsOnly, true },
        { ListeningState.ListeningForTaskMenuCommandsOnly, true },
    };

    private static Dictionary<ListeningState, bool> validMenuActivationStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true },
        { ListeningState.ListeningForMenuActivationCommandsOnly, true },
    };

    public bool CurrentStateIsAllowedInDictionary(Dictionary<ListeningState, bool> dictToSearch) {
        // Go through every enum value
        foreach (ListeningState state in Enum.GetValues(typeof(ListeningState)))
        {
            if (!dictToSearch.ContainsKey(state))
            {
                // If the state is not in the dictionary, add it and set it to false
                dictToSearch[state] = false;
            }
        }
        // If the current state is in the dictionary, return true or false depending on if it is allowed
        return dictToSearch[currentListeningState];
    }

    public bool RecitingWordsIsAllowed()
    {
        // If the current state is in the dictionary, return true or false depending on if it is allowed
        // This prevents the accidental activation of reciting logic when the user is in a menu or elsewhere
        return CurrentStateIsAllowedInDictionary(validRecitingStates);
    }

    public void ReactivateToTryMenuNavigationCommandsAgain() {
        // This is called when the user says something that is not a valid menu navigation command
        // We want to reactivate the wit listening state manager to try again
        // This is a coroutine because we want to wait a few seconds before reactivating
        StartCoroutine(ReactivateToTryMenuNavigationCommandsAgainCoroutine());
    }

    IEnumerator ReactivateToTryMenuNavigationCommandsAgainCoroutine() {
        // Wait a few seconds before reactivating
        yield return new WaitForSeconds(1);
        // Reactivate
        TransitionToRelevantMenuNavigationStateBasedOnLevel();
    }

    public bool MenuNavigationCommandsAreAllowed() {
        return CurrentStateIsAllowedInDictionary(validMenuNavigationStates);
    }

    public bool MenuActivationCommandsAreAllowed() {
        return CurrentStateIsAllowedInDictionary(validMenuActivationStates);
    }

    public void TransitionToRelevantMenuNavigationStateBasedOnLevel() {
        if (scene != "Level3") {
            TransitionToState(ListeningState.ListeningForTaskMenuCommandsOnly);
        } else {
            TransitionToState(ListeningState.ListeningForLobbyMenuCommandsOnly);
        }
    }
    private void Start()
    {        
        scene = SceneManager.GetActiveScene().name;
        if (scene == "Level3") {
             TransitionToState(ListeningState.ListeningForEverything);
        } else {
            TransitionToState(ListeningState.ListeningForMenuActivationCommandsOnly);
        }
    }

    void Update() {
        // update the states based on keypresses 1-7
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            TransitionToState(ListeningState.ListeningForEverything);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            TransitionToState(ListeningState.ListeningForMenuActivationCommandsOnly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            TransitionToState(ListeningState.ListeningForRecitedWordsOnly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            TransitionToState(ListeningState.ListeningForTaskMenuCommandsOnly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            TransitionToState(ListeningState.ListeningForConfirmation);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            TransitionToState(ListeningState.ListeningForLobbyMenuCommandsOnly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            TransitionToState(ListeningState.NotListening);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(CurrentStateIsAllowedInDictionary(validRecitingStates));
        }

    }

    public void StoppedListening() {
        Debug.LogError("Stopped! Reactivating");
        StartCoroutine(ResetToCurrentListeningState());
    }

    public IEnumerator ResetToCurrentListeningState() {
           
        // Turn it back on again
        // TODO - improve this - I don't know why it doesn't work without the delay
        yield return new WaitForSeconds(0.00001f);
        TransitionToState(currentListeningState);
        
    }

    public IEnumerator TurnWitOffAndOn(Wit wit) {
        // Turn it off and on
        wit.Deactivate();
        yield return new WaitForSeconds(0.00001f);
        wit.Activate();
    }

    void DisableOtherWitsAndEnableThisOne(string witToEnable) {
         for (int i = 0; i < wits.Length; i++)
            {  
                Wit wit = wits[i].GetComponent<Wit>();
                 if (wits[i].name == witToEnable) {
                      wits[i].SetActive(true);
                      StartCoroutine(TurnWitOffAndOn(wit));
                 } else {
                      wits[i].SetActive(false);
                 } 
            }
    }

    void DisableAllWits() {
         for (int i = 0; i < wits.Length; i++)
            {  
                wits[i].SetActive(false);
            }
    }
 
    // This is called from the WitListeningStateMachine script using actual enum values.

    public void TransitionToState(ListeningState nextState)
        {
            listeningText3D.UpdateText(nextState.ToString());

            switch (nextState)
            {
                case ListeningState.NotListening:
                    DisableAllWits();
                    break;
                case ListeningState.ListeningForMenuActivationCommandsOnly:
                    DisableOtherWitsAndEnableThisOne("MenuListeningWit");
                    break;
                case ListeningState.ListeningForEverything:

                    DisableOtherWitsAndEnableThisOne("EverythingWit");
                    break;
                case ListeningState.ListeningForTaskMenuCommandsOnly:
                    DisableOtherWitsAndEnableThisOne("TaskMenuNavigationWit");
                    break;
                case ListeningState.ListeningForConfirmation:
                    Debug.Log("Starting confirmation: " + nextState);

                     DisableOtherWitsAndEnableThisOne("ConfirmationWit");
                    break;
                case ListeningState.ListeningForLobbyMenuCommandsOnly:
                    break;
                default:
                    Debug.LogError("Invalid state transition." + nextState);
                    return;
            }

            currentListeningState = nextState;

            Debug.Log("WitListeningStateMachine transitioned to state: " + nextState);
        }
}
