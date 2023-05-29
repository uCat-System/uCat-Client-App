using System;
using UnityEngine;
using MText;
using Meta.WitAi;


public class WitListeningStateManager : MonoBehaviour
{
    public Modular3DText listeningText3D;

    public WitListeningStateMachine witListeningStateMachine;
    public Wit witModule;

    private void Awake()
    {
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
            witModule.Activate();
        }

        public void DeactivateWit()
        {
            witModule.Deactivate();
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
