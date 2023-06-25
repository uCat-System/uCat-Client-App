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
            yield break;
        }

        partialText3D.UpdateText("");
        reciteText3D.Material = defaultColour;
        string word = activeList[currentWordOrSentenceIndex];
        // word == "Hello"
    
        for (float i = 0; i < 3; i++)
        {

        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly) {
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
            yield break;
        } else {
            subtitleText3D.UpdateText("");
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
        if (_witListeningStateManager.currentListeningState == EState.ListeningForTaskMenuCommandsOnly
        || _witListeningStateManager.currentListeningState == EState.ListeningForMenuActivationCommandsOnly)
        {
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
        if (currentWordOrSentenceIndex < activeList.Count - 1)
        {
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
        MoveOnIfMoreWordsInList();
    }


    IEnumerator CheckWordListStatus()
    {
        // Either proceed to next word list (ui), or end the game.
        if (wordListComplete && !uiComplete)
        { 
            activeList = currentUiList;
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Great! Moving onto UI word list.");

            yield return new WaitForSeconds(2);
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
            Debug.LogError("Something went wrong in conditional for list changing.");
        }
    }

    void GameOver()
    {
        _scoreManager.DisplayScoreInPartialTextSection();
        reciteText3D.UpdateText("Say 'next' to proceed.\nOr 'repeat' to repeat sentences.");
        isDeciding = true;
    }
}
