using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using MText;
using EState = WitListeningStateMachine.State;

public class WordReciteManager : MonoBehaviour
{
    public bool isDeciding = false;
    public bool resuming = false;

    // Current word tracking
    int currentWordOrSentenceIndex;

    // Word lists
    private List<string> currentWordOrSentenceList;

    private List<string> currentUiList;

    // This is the list that is active, either words/sentences OR UI list
    private List<string> activeList;

    // Scriptable asset for externally stored word lists
    public WordLists wordLists;

    // Track if the lists have been completed
    bool wordListComplete;
    
    bool uiComplete;

    bool openQuestionsComplete;

    // Text colours

    public Material correctColour;
    public Material incorrectColour;
    public Material defaultColour;
    public Material listeningColour;

    // External Managers

    public FreeSpeechManager _freeSpeechManager;

    public  WitListeningStateManager _witListeningStateManager;

    public ScoreManager _scoreManager;
    public LevelManager _levelManager;

    public Modular3DText reciteText3D;
    public Modular3DText partialText3D;

    public Modular3DText subtitleText3D;

    public AudioSource reciteBoardAudioSource;

    [SerializeField] private Wit wit;
    public AudioClip[] wordSounds;

    void Awake()
    {
        // Assigning gameobjects
        wit = GameObject.FindWithTag("Wit").GetComponent<Wit>();
        reciteBoardAudioSource = GameObject.FindWithTag("ReciteBoard").GetComponent<AudioSource>();
        subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
       
        // Game state variables
        uiComplete = false;
        wordListComplete = false;

        // Initialise Word Lists
        SetWordAndUiListsBasedOnLevel();
        activeList = currentWordOrSentenceList;
        currentWordOrSentenceIndex = 0;

        // Set score based on amount of words in lists
        if (_levelManager.currentLevel != "Level3")
        {
            _scoreManager.SetMaxScoreBasedOnWordListCount(currentWordOrSentenceList.Count + currentUiList.Count);
        }

        // Start the first word
        reciteText3D.Material = defaultColour;
        StartCoroutine(StartCurrentWordCountdown());
    }

    void SetWordAndUiListsBasedOnLevel() {
        switch (_levelManager.currentLevel) {
            case "Level1":
                currentWordOrSentenceList = wordLists.level1WordList;
                currentUiList = wordLists.level1UiList;
                break;
            case "Level2":
                currentWordOrSentenceList = wordLists.level2SentenceList;
                currentUiList = wordLists.level2UiList;
                break;
            case "Level3":
                currentWordOrSentenceList = wordLists.level3OpenQuestionsList;
                break;
        }
    }

    public IEnumerator StartCurrentWordCountdown()
    {
        // Clear text
        subtitleText3D.UpdateText("");

        // Play sound

        reciteBoardAudioSource.clip = wordSounds[0];
        reciteBoardAudioSource.Play();

        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly)
        {
            Debug.Log("Breaking out of countdown because in navigation state");
            yield break;
        }

        partialText3D.UpdateText("");
        reciteText3D.Material = defaultColour;
        Debug.Log("Abnoput to display index " + currentWordOrSentenceIndex + " of " + currentWordOrSentenceList);
        string word = activeList[currentWordOrSentenceIndex];
        // word == "Hello"
    
        for (float i = 0; i < 3; i++)
        {

        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly) {
            Debug.Log("Breaking out of countdown because in navigation state");
            yield break;
        }
            switch (i)
            {
                case 0:
                    reciteText3D.UpdateText("..." + word + "...");
                    break;
                case 1:
                    reciteText3D.UpdateText(".." + word + "..");
                    break;
                case 2:
                    reciteText3D.UpdateText("." + word + ".");

                    // Discard anything said during countdown and start fresh
                    // _witListeningStateManager.TransitionToState(EState.NotListening);
                    break;
            }
           
            yield return new WaitForSeconds(1);
        }

        // Countdown finished, start listening for the word
        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly) {
             Debug.Log("EXITING OUT BECAUSE WE ARE IN MENU STATE " + _witListeningStateManager.currentListeningState);
            yield break;
        } else {
            subtitleText3D.UpdateText("");
            Debug.Log("CONTINUING, STATE IS " + _witListeningStateManager.currentListeningState);
            _witListeningStateManager.TransitionToState(EState.ListeningForEverything);
            reciteText3D.UpdateText(word);
            reciteText3D.Material = listeningColour;
        }

    }

    public void OnMicrophoneTimeOut()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    IEnumerator ChangeTimeOutText()
    {
        reciteText3D.UpdateText("Timed out! Moving on...");
        yield return new WaitForSeconds(2);
        GoToNextWord();
    }

    public void OnMicrophoneInactivity()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }
    void Update() {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     GoToNextWord();
        // }
    }

    public void GoToNextWord()
    {
        _witListeningStateManager.TransitionToState(EState.ListeningForMenuActivationCommandsOnly);
        // If the next word does not exceed the limit
        if (currentWordOrSentenceIndex < currentWordOrSentenceList.Count-1)
        {
            currentWordOrSentenceIndex++;
        }
        
        StartCoroutine(StartCurrentWordCountdown());

    }
    public void RepeatSameWord()
    {
        _witListeningStateManager.TransitionToState(EState.ListeningForMenuActivationCommandsOnly);
        StartCoroutine(StartCurrentWordCountdown());
    }
    public void StartWordCheck(string transcription)
    {
        StartCoroutine(CheckRecitedWord(transcription));
    }
   
    public IEnumerator CheckRecitedWord(string text)
    {
        Debug.Log("Checking recited word with state of " + _witListeningStateManager.currentListeningState);
        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly
        || _witListeningStateManager.currentListeningState == EState.ListeningForMenuActivationCommandsOnly)
        {
            Debug.Log("Breaking out of recited word because menu active or in command mode" + _witListeningStateManager.currentListeningState);
            yield break;
        }
        bool wordAnsweredCorrectly;

         if (isDeciding) {
            // Are they saying 'next' or 'repeat'
            if (text.ToLower() == "next")
            {
                _levelManager.LevelComplete();
                yield break;
            }
            else if (text.ToLower() == "repeat")
            {
                _levelManager.RepeatLevel();
                yield break;

            }

            else {
                Debug.Log("Not next or repeat");
                partialText3D.UpdateText("I didn't understand that, please try again.");
                GameOver();
                yield break;

            }
        }

        // Does their answer match the current word?
        wordAnsweredCorrectly = text.ToLower() == activeList[currentWordOrSentenceIndex].ToLower();

        // Change text to reflect correct / incorrect 

        if (wordAnsweredCorrectly) {
            reciteText3D.UpdateText("Correct!");
            reciteText3D.Material = correctColour;
            // reciteBoardAudioSource.clip = wordSounds[1];
        } else {
            reciteText3D.UpdateText("Incorrect.");
            reciteText3D.Material = incorrectColour;
            // reciteBoardAudioSource.clip = wordSounds[2];
        }

        // Play sound
        // reciteBoardAudioSource.Play();

        yield return new WaitForSeconds(2);

        if (wordAnsweredCorrectly)
        {
            Debug.Log("Word answered correctly");
            WordAnsweredCorrectly();
        }
        else
        {
            StartCoroutine(WordAnsweredIncorrectly());
        }
    }

    void AddScoreToScoreManager()
    {
         if (_levelManager.currentLevel == "Level1")
        {
            _scoreManager.Level1CurrentScore = _scoreManager.Level1CurrentScore + 1;
        }
        else if (_levelManager.currentLevel == "Level2")
        {
            _scoreManager.Level2CurrentScore = _scoreManager.Level2CurrentScore + 1;
        }
    }

    IEnumerator WordAnsweredIncorrectly()
    {
        reciteText3D.UpdateText("Try again!");
        yield return new WaitForSeconds(1);
        RepeatSameWord();
    }

    public void MoveOnIfMoreWordsInList ()
    {
        Debug.Log("checking if more words in list" + currentWordOrSentenceIndex + " " + activeList.Count);
        if (currentWordOrSentenceIndex < activeList.Count - 1)
        {
            Debug.Log("going to next word because index valid");
            GoToNextWord();
        }

        else
        {
            // Mark the current list as complete, and move on to the next (if any) via changing booleans
            _witListeningStateManager.TransitionToState(EState.ListeningForEverything);
            if (activeList == currentWordOrSentenceList) { wordListComplete = true; }
            if (activeList == currentUiList) { uiComplete = true; }
            if (_levelManager.currentLevel == "Level3") { openQuestionsComplete = true; }
            StartCoroutine(CheckWordListStatus());
        }
    }
    void WordAnsweredCorrectly()
    {
        AddScoreToScoreManager();
        Debug.Log("moving on");

        MoveOnIfMoreWordsInList();
    }


    IEnumerator CheckWordListStatus()
    {
        Debug.Log("Checking word list status");
        Debug.Log("Chang complete: " + wordListComplete);
        Debug.Log("UI complete: " + uiComplete);
        Debug.Log("openQuestionsComplete: " + openQuestionsComplete);

        // Either proceed to next word list (ui), or end the game.
        if (wordListComplete && !uiComplete)
        { 
            activeList = currentUiList;
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Great! Moving onto UI word list.");

            yield return new WaitForSeconds(2);
            Debug.Log("Changing to UI list");
            Debug.Log("fierst word should be " + currentUiList[0]);
                        Debug.Log("actual first:  " + activeList[0]);

            StartCoroutine(StartCurrentWordCountdown());
            
        }
        // If level 1/2 have both lists done, or level 3 has open questions done, end the game.
        else if (wordListComplete && uiComplete || openQuestionsComplete)
        {
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Finished!");

            GameOver();
        }

        else
        {
            Debug.Log("Something went wrong in conditional for list changing.");
        }
    }

    void GameOver()
    {
        _scoreManager.DisplayScoreInPartialTextSection();
        reciteText3D.UpdateText("Say 'next' to proceed.\nOr 'repeat' to repeat sentences.");
        // StartCoroutine(_witListeningStateManager.TurnWitOffAndOn());
        isDeciding = true;
    }
}
