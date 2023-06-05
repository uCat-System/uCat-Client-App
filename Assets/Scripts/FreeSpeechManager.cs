using System.Collections;
using UnityEngine;
using Meta.WitAi;
using MText;
using Meta.WitAi.Lib;
using Meta.WitAi.Configuration;
using UnityEngine.SceneManagement;

namespace MText
{ 
    public class FreeSpeechManager : MonoBehaviour
    {


/*

    1) Word 'hello' appears on screen (reciteText) in grey, with ... on the left and 
        right of it ("...hello...")

    2) Every second, the dots either side disappear

    3) When the dots are all gone, the word turns yellow and the microphone activates
        --> Fire event to start listening and update text

    4) The user says the word

    5) The microphone deactivates and the word turns red if wrong, green if correct
        --> Fire event to stop listening and update text



    6) Next word starts from step 1 After a brief delay (1-2 sec?)

*/

        [SerializeField] private Wit wit;
        private UIManager uiManager;

        public WordReciteManager wordReciteManager;
        public Modular3DText partialText3D;
        public Modular3DText fullText3D;

        public WitListeningStateManager _witListeningStateManager;

        public string cachedText = "";

        Scene scene;

        void Start()
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            scene = SceneManager.GetActiveScene();
        }
        public void MicActivityDetected()
        {
            //Debug.Log("Mic activity!");
        }
    
        public void StoppedListeningDueToInactivity()
        {
            HandleInactivityFailure();
        }

        public void StoppedListeningDueToTimeout()
        {
            HandleInactivityFailure();
        }
        public void StoppedListening()
        {
            // Debug.Log("Stopped!");
        }

        // public IEnumerator StartListeningAgain()
        // {
        //     yield return new WaitForSeconds(0.00001f);
        //     wit.Activate();
        // }

        public void HandlePartialTranscription(string text)
        {
            Debug.Log("Receiving partial text of " + text);
            if (_witListeningStateManager.currentListeningState == "ListeningForEverything") {
                partialText3D.UpdateText(text);
            }
        }

        public void HandleFullTranscription(string text)
        {
            Debug.Log("Receiving full text of " + text);

            // 1) Always listen for menu
            uiManager.CheckIfUICommandsWereSpoken(text.ToLower());

            bool isInReciteMode = _witListeningStateManager.currentListeningState == "ListeningForEverything" ||
            _witListeningStateManager.currentListeningState == "ListeningForRecitedWordsOnly";

            // 2) Activate Tasks if in recite mode
            if (isInReciteMode)
                {
                    Debug.Log("activating word task");
                    ActivateTasksBasedOnTranscription(text);
                }
                else {
                    Debug.Log("WRONG state - did not activate word task");
                }
        }

        void HandleInactivityFailure()
        {
            wordReciteManager.OnMicrophoneTimeOut();
        }

        public void ActivateTasksBasedOnTranscription(string text)
        {        
            // Update the spoken text
            CalculateCachedText(text);
            fullText3D.UpdateText(cachedText);

            if (SceneManager.GetActiveScene().name != "Level3") 
            {
                Debug.Log("activating word task");
                wordReciteManager.StartWordCheck(text);
          
            } else {
                _witListeningStateManager.ChangeState("ListeningForEverything");
            }


        }

     void CalculateCachedText(string newText) {
        // Prevent the text log becoming too long
        int maxLengthBasedOnScene = 0;
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Level1":
                maxLengthBasedOnScene = 40;
                break;
            case "Level2":
                 maxLengthBasedOnScene = 70;
                 break;
            case "Level3":
                maxLengthBasedOnScene = 120;
                break;
            default:
                maxLengthBasedOnScene = 30;
                break;
        }

        if (cachedText.Length > maxLengthBasedOnScene) {
            cachedText = newText;
        } else {
            cachedText = cachedText + '\n' + ' ' + newText;
        }
    }
    }

   

}
