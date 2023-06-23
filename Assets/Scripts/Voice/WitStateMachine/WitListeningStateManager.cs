using System;
using UnityEngine;
using MText;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using System.Collections;
using EState = WitListeningStateMachine.State;




public class WitListeningStateManager : MonoBehaviour
{
    public Modular3DText listeningText3D;
    public UIManager _uiManager;
    public EState currentListeningState;
    public WordReciteManager _wordReciteManager;

    public WitListeningStateMachine _witListeningStateMachine;
    public Wit _everythingWit;
    public Wit _menuListeningWit;

    public GameObject[] wits;

    private void Start()
    {
        _wordReciteManager = GameObject.FindWithTag("WordReciteManager")?.GetComponent<WordReciteManager>();

        if (_wordReciteManager == null)
        {
            _wordReciteManager = null;
        }


        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Level3") {
             TransitionToState(EState.ListeningForEverything);
        } else {
            TransitionToState(EState.ListeningForMenuActivationCommandsOnly);
        }

        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        listeningText3D = GameObject.FindWithTag("ListeningText3D").GetComponent<Modular3DText>();
        if (_witListeningStateMachine == null)
        {
            Debug.LogError("WitListeningStateMachine is not assigned.");
            return;
        }
    }

       public void ActivateWit()
        {
            // Have to manually check if it exists since we turn Wit on and off 
            GameObject witGameObject = GameObject.FindWithTag("Wit");
            if (witGameObject != null && witGameObject.activeSelf)
            {
                _everythingWit = witGameObject.GetComponent<Wit>();
                
                if (_everythingWit != null)
                {
                    Debug.Log("Wit module found, activating");
                    _everythingWit.Activate();
                }
            }

        }

    public void DetectUICommandsInWitListeningStateManager(string text) {
        // if the state is ListeningForMenuActivationCommandsOnly OR ListeningForEverything,
        // check if the spoken text is in the menuCommandPhrases list:
        if (currentListeningState == WitListeningStateMachine.State.ListeningForMenuActivationCommandsOnly ||
            currentListeningState == WitListeningStateMachine.State.ListeningForEverything)
        {
            _uiManager.CheckIfUICommandsWereSpoken(text);
        }
    }

    public void StoppedListening() {
        Debug.Log("Stopped! Reactivating");
        StartCoroutine(ResetToCurrentListeningState());
    }

    public IEnumerator ResetToCurrentListeningState() {
           
        // Turn it back on again
        // TODO - improve this - I don't know why it doesn't work without the delay
        yield return new WaitForSeconds(0.00001f);
        // change to the previous state (either menu only or all)
        TransitionToState(currentListeningState);
        
    }

    void DisableOtherWitsAndEnableThisOne(string witToEnable) {
         for (int i = 0; i < wits.Length; i++)
            {  
                Debug.Log("wits[i].name " + wits[i].name);
                Debug.Log("witToEnable " + witToEnable);
                Wit wit = wits[i].GetComponent<Wit>();
                 if (wits[i].name == witToEnable) {
                      wits[i].SetActive(true);
                      wit.Activate();
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

    public void TransitionToState(WitListeningStateMachine.State nextState)
        {
            //TODO Use import statements to call enum values
            listeningText3D.UpdateText(nextState.ToString());

            switch (nextState)
            {
                case WitListeningStateMachine.State.NotListening:
                    // DisableAllWits();
                    break;
                case WitListeningStateMachine.State.ListeningForMenuActivationCommandsOnly:
                    DisableOtherWitsAndEnableThisOne("MenuListeningWit");
                    break;
                case WitListeningStateMachine.State.ListeningForEverything:
                    DisableOtherWitsAndEnableThisOne("EverythingWit");
                    break;
                case WitListeningStateMachine.State.ListeningForTaskMenuCommandsOnly:
                    Debug.Log("Should be disabling/enabling");
                    DisableOtherWitsAndEnableThisOne("TaskMenuNavigationWit");
                    break;
                case WitListeningStateMachine.State.ListeningForConfirmation:
                     DisableOtherWitsAndEnableThisOne("ConfirmationWit");
                    break;
                case WitListeningStateMachine.State.ListeningForLobbyMenuCommandsOnly:
                    break;
                default:
                    Debug.LogError("Invalid state transition." + nextState);
                    return;
            }

            currentListeningState = nextState;

            Debug.Log("WitListeningStateMachine transitioned to state: " + nextState);
        }

    // This can be called with a string value from external scripts.
    // public void ChangeState(string state)
    // {
    //     if (Enum.TryParse(state, out WitListeningStateMachine.State newState))
    //     {
    //         TransitionToState(newState);
    //         currentListeningState = state;
    //     }
    //     else
    //     {
    //         Debug.LogError("Invalid state: " + state);
    //     }
    // }
}
