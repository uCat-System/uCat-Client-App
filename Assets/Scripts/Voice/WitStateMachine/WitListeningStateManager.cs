using System;
using UnityEngine;
using MText;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public UIManager _uiManager;
    public ListeningState currentListeningState;
    public WordReciteManager _wordReciteManager;

    public GameObject[] wits;

    private void Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Level3") {
             TransitionToState(ListeningState.ListeningForEverything);
        } else {
            TransitionToState(ListeningState.ListeningForMenuActivationCommandsOnly);
        }
    }

    public void DetectUICommandsInWitListeningStateManager(string text) {
        // if the state is ListeningForMenuActivationCommandsOnly OR ListeningForEverything,
        // check if the spoken text is in the menuCommandPhrases list:
        if (currentListeningState == ListeningState.ListeningForMenuActivationCommandsOnly ||
            currentListeningState == ListeningState.ListeningForEverything)
        {
            _uiManager.CheckIfUICommandsWereSpoken(text);
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
        Debug.Log("Deactivating and reactivating wit: " + wit.name);
        wit.Deactivate();
        yield return new WaitForSeconds(0.00001f);
        wit.Activate();
    }

    void DisableOtherWitsAndEnableThisOne(string witToEnable) {
            Debug.Log("witToEnable: " + witToEnable);
            Debug.Log("wits.Length: " + wits.Length);
            Debug.Log("wits[0]: " + wits[0]);
         for (int i = 0; i < wits.Length; i++)
            {  
                Debug.Log("wits[i]: " + wits[i]);

                Wit wit = wits[i].GetComponent<Wit>();
                 if (wits[i].name == witToEnable) {
                    Debug.Log("Found");
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
