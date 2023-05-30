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

    public WitListeningStateMachine witListeningStateMachine;
    public Wit witModule;

    private void Start()
    {
        ChangeState("ListeningForEverything");
        _uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        witModule = GameObject.FindWithTag("Wit").GetComponent<Wit>();
        listeningText3D = GameObject.FindWithTag("ListeningText3D").GetComponent<Modular3DText>();
        if (witListeningStateMachine == null)
        {
            Debug.LogError("WitListeningStateMachine is not assigned.");
            return;
        }
    }

       public void ActivateWit()
        {
            Debug.Log("Wit activated.");
            witModule.Deactivate();
            witModule.Activate();
        }

        public void DeactivateWit()
        {
            Debug.Log("Wit deactivated.");
            witModule.Deactivate();
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

    private IEnumerator StartListeningAgain() {
        yield return new WaitForSeconds(0.00001f);
        ChangeState("ListeningForEverything");
    }

    // This is called from the WitListeningStateMachine script using actual enum values.

    public void TransitionToState(WitListeningStateMachine.State nextState)
        {
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
                default:
                    Debug.LogError("Invalid state transition.");
                    return;
            }

            witListeningStateMachine.currentState = nextState;
            listeningText3D.UpdateText(nextState.ToString());

            Debug.Log("WitListeningStateMachine transitioned to state: " + nextState);
        }

    // This can be called with a string value from external scripts.
    public void ChangeState(string state)
    {
        if (Enum.TryParse(state, out WitListeningStateMachine.State newState))
        {
            TransitionToState(newState);
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
    }
}
