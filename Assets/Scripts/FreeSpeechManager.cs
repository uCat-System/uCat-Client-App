using System.Collections;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.SceneManagement;

namespace MText
{ 
    public class FreeSpeechManager : MonoBehaviour
    {
        [SerializeField] private Wit wit;
        private UIManager uiManager;

        public WordReciteManager wordReciteManager;
        public Modular3DText partialText3D;
        public Modular3DText subtitleText3D;
        public Modular3DText fullText3D;

        public WitListeningStateManager _witListeningStateManager;

        public string cachedText = "";

        Scene scene;

        void Start()
        {
            subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            scene = SceneManager.GetActiveScene();
        }
    
        public void StoppedListeningDueToInactivity()
        {
            Debug.Log("Stopped listening due to inactivity");
            HandleInactivityFailure();
        }

        public void StoppedListeningDueToDeactivation()
        {
            Debug.Log("Stopped listening due to Deactivation");
            // HandleInactivityFailure();
        }

        public void StoppedListeningDueToTimeout()
        {
            Debug.Log("Stopped listening due to timeout");
            HandleInactivityFailure();
        }

        public void HandlePartialTranscription(string text)
        {
            Debug.Log("Partial received: " + text);
            // Always update subtitles when attempting speech
            subtitleText3D.UpdateText(text);
            if (_witListeningStateManager.currentListeningState == "ListeningForEverything") {
                partialText3D.UpdateText(text);
            }
        }

         public void MinimumWakeThresholdHit()
        {
            Debug.Log("HIT MINIMUM: ");
            
        }

        public void HandleFullTranscription(string text)
        {
            Debug.Log("Receiving full text of " + text);

            // Clear subtitle speech
            // 1) Always listen for menu
            uiManager.CheckIfUICommandsWereSpoken(text.ToLower());

            bool isInReciteMode = _witListeningStateManager.currentListeningState == "ListeningForEverything" ||
            _witListeningStateManager.currentListeningState == "ListeningForRecitedWordsOnly";

            // 2) Activate Tasks if in recite mode
            if (isInReciteMode)
                {
                    Debug.Log("activating word task or free recite");
                    ActivateTasksBasedOnTranscription(text);
                }
                else {
                    Debug.Log("WRONG state - did not activate word task");
                }
        }

        public void HandleInactivityFailure()
        {
            wordReciteManager.OnMicrophoneTimeOut();
        }

        public void ActivateTasksBasedOnTranscription(string text)
        {        
            if (SceneManager.GetActiveScene().name != "Level3") 
            {
                Debug.Log("activating word task");
                wordReciteManager.StartWordCheck(text);
          
            } else {
                Debug.Log("should be transcribing");
                 // Update the spoken text
                CalculateCachedText(text);
                fullText3D.UpdateText(cachedText);
                StartCoroutine(_witListeningStateManager.ResetToCurrentListeningState());
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
