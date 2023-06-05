using System;
using UnityEngine;
using MText;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class WitListeningStateManager : MonoBehaviour
{
    public Modular3DText listeningText3D;
    public UIManager _uiManager;
    public string currentListeningState;
    public WordReciteManager _wordReciteManager;

    public WitListeningStateMachine witListeningStateMachine;
    public Wit witModule;

    private void Start()
    {
        _wordReciteManager = GameObject.FindWithTag("WordReciteManager")?.GetComponent<WordReciteManager>();

        if (_wordReciteManager == null)
        {
            _wordReciteManager = null;
        }


        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Level3") {
             ChangeState("ListeningForEverything");
        } else {
            ChangeState("ListeningForMenuCommandsOnly");
        }
        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        listeningText3D = GameObject.FindWithTag("ListeningText3D").GetComponent<Modular3DText>();
        if (witListeningStateMachine == null)
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
                witModule = witGameObject.GetComponent<Wit>();
                
                if (witModule != null)
                {
                    witModule.Activate();
                }
            }

            Debug.Log("Wit activated.");
        }

        public void DeactivateWit()
        {
           GameObject witGameObject = GameObject.FindWithTag("Wit");
            if (witGameObject != null && witGameObject.activeSelf)
            {
                witModule = witGameObject.GetComponent<Wit>();
                
                if (witModule != null)
                {
                    witModule.Deactivate();
                }
            }

            Debug.Log("Wit dectivate called, but still active");
        }

    public void DetectUICommandsInWitListeningStateManager(string text) {
        // if the state is ListeningForMenuCommandsOnly OR ListeningForEverything,
        // check if the spoken text is in the menuCommandPhrases list:
        if (witListeningStateMachine.currentState == WitListeningStateMachine.State.ListeningForMenuCommandsOnly ||
            witListeningStateMachine.currentState == WitListeningStateMachine.State.ListeningForEverything)
        {
            _uiManager.CheckIfUICommandsWereSpoken(text);
        }
    }

    public void StoppedListening() {
        Debug.Log("Stopped!");
        StartCoroutine(StartListeningAgain());
    }

    public IEnumerator StartListeningAgain() {
           
        // Turn it back on again
        // TODO - improve this - I don't know why it doesn't work without the delay
        yield return new WaitForSeconds(0.00001f);
        Debug.Log("Starting again, listening for ." + currentListeningState);
        // change to the previous state (either menu only or all)
        ChangeState(currentListeningState);
        
    }

    // This is called from the WitListeningStateMachine script using actual enum values.

    public void TransitionToState(WitListeningStateMachine.State nextState)
        {
            listeningText3D.UpdateText(nextState.ToString());

            switch (nextState)
            {
                case WitListeningStateMachine.State.NotListening:
                    DeactivateWit();
                    break;
                case WitListeningStateMachine.State.ListeningForMenuCommandsOnly:
                    ActivateWit();
                    // Additional logic specific to this state
                    break;
                case WitListeningStateMachine.State.ListeningForEverything:
                    ActivateWit();
                    // Additional logic specific to this state
                    break;
                case WitListeningStateMachine.State.ListeningForRecitedWordsOnly:
                    ActivateWit();
                    // Additional logic specific to this state
                    break;
                case WitListeningStateMachine.State.ListeningForNavigationCommandsOnly:
                    ActivateWit();
                    // Additional logic specific to this state
                    break;
                default:
                    Debug.LogError("Invalid state transition." + nextState);
                    return;
            }

            witListeningStateMachine.currentState = nextState;

            Debug.Log("WitListeningStateMachine transitioned to state: " + nextState);
        }

    // This can be called with a string value from external scripts.
    public void ChangeState(string state)
    {
        if (Enum.TryParse(state, out WitListeningStateMachine.State newState))
        {
            TransitionToState(newState);
            currentListeningState = state;
        }
        else
        {
            Debug.LogError("Invalid state: " + state);
        }
    }

    void Update() {
        // check keypresses for 1,2,3 and 4, and call ChangeState with the appropriate strings:
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState("NotListening");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState("ListeningForMenuCommandsOnly");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeState("ListeningForEverything");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeState("ListeningForRecitedWordsOnly");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeState("ListeningForNavigationCommandsOnly");
        }
    }
}
