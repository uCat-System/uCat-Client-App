using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MText;
using EListeningState = WitListeningStateManager.ListeningState;
using EProceedResponseType = ConfirmationHandler.ProceedResponseType;
using ECorrectResponseType = CheckRecitedWordHandler.CorrectResponseType;

public class WordReciteManager : MonoBehaviour
{
    public AudioSource catAudioSource;
    
    public AnimationDriver catAnimationDriver;

    public Modular3DText dialogueText3D;
    
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

    public AudioClip[] wordSounds;

    void Start()
    {
        // Assigning gameobjects
        reciteBoardAudioSource = GameObject.FindWithTag("ReciteBoard").GetComponent<AudioSource>();
        subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
        reciteText3D = GameObject.FindWithTag("ReciteText3D").GetComponent<Modular3DText>();
        reciteBoardAudioSource = GameObject.FindWithTag("ReciteBoard").GetComponent<AudioSource>();

       
        // Game state variables
        uiComplete = false;
        wordListComplete = false;

        // Initialise Word Lists
        SetWordAndUiListsBasedOnLevel();
        activeList = currentWordOrSentenceList;
        Debug.Log("active list is " + activeList.Count);

        currentWordOrSentenceIndex = 0;
        Debug.Log("current index is " + currentWordOrSentenceIndex);

        // Set score based on amount of words in lists
        if (_levelManager.currentLevel == "Level1" || _levelManager.currentLevel == "Level2")
        {
            _scoreManager.SetMaxScoreBasedOnWordListCount(currentWordOrSentenceList.Count + currentUiList.Count);
        }

        // Start the first word
        reciteText3D.Material = defaultColour;
        
        StartCoroutine(StartCurrentWordCountdown());
    }

    void SetWordAndUiListsBasedOnLevel() {
        switch (_levelManager.currentLevel) {
            case "Intro":
                currentWordOrSentenceList = wordLists.introWordList;
                currentUiList = null;
                break;
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

        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);

        // Clear text
        subtitleText3D.UpdateText("");

        // Play sound

        reciteBoardAudioSource.clip = wordSounds[0];
        reciteBoardAudioSource.Play();

        if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForTaskMenuCommandsOnly)
        {
            yield break;
        }

        partialText3D.UpdateText("");
        reciteText3D.Material = defaultColour;
        string word = activeList[currentWordOrSentenceIndex];
        // word == "Hello"
    
        for (float i = 0; i < 3; i++)
        {

        if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForTaskMenuCommandsOnly) {
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
                    _witListeningStateManager.TransitionToState(EListeningState.NotListening);
                    break;
            }
           
            yield return new WaitForSeconds(1);
        }

        // Countdown finished, start listening for the word
        if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForTaskMenuCommandsOnly) {
            yield break;
        } else {
            subtitleText3D.UpdateText("");
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForEverything);
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
        GoToNextWordIfItExists();
    }

    public void OnMicrophoneInactivity()
    {
        // Do not add to score
        StartCoroutine(ChangeTimeOutText());
    }

    public void GoToNextWordIfItExists()
    {
        // If the next word does not exceed the limit
        if (currentWordOrSentenceIndex < activeList.Count-1)
        {
            currentWordOrSentenceIndex++;
            Debug.Log("going to next word " + currentWordOrSentenceIndex);
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
            StartCoroutine(StartCurrentWordCountdown());
        } else {
            // End of list
            GameOver();
        }
    }
    public void RepeatSameWord()
    {
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
        StartCoroutine(StartCurrentWordCountdown());
    }
    public void StartWordCheck(string transcription)
    {
        StartCoroutine(CheckRecitedWord(transcription));
    }
   
    public IEnumerator CheckRecitedWord(string text)
    {
        if (!_witListeningStateManager.RecitingWordsIsAllowed()) {
            // Menu
            yield break;
        }

        // Compare the uttered text with the correct text
        ECorrectResponseType correctResponseType = CheckRecitedWordHandler.CheckIfWordOrSentenceIsCorrect(text, activeList[currentWordOrSentenceIndex]);
        StartCoroutine(HandleWordOrSentenceCorrectOrIncorrect(correctResponseType));
    }
    
    public IEnumerator HandleWordOrSentenceCorrectOrIncorrect(ECorrectResponseType responseType) {
        string correctResponseText = CheckRecitedWordHandler.correctResponses[responseType];
        dialogueText3D.UpdateText(correctResponseText);

        switch (responseType) {
            case ECorrectResponseType.POSITIVE_CORRECT_RESPONSE:
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
// ** TODO: Refactor to the Dialogue management paradigm (instead of directly envoking the assets) **
                //Playing uCat monologue when correct answer is found
                catAudioSource.PlayOneShot(ConfirmationHandler.temp_audioConfPos);
                //dialogueText.UpdateText("uCat: " + correctResponseText);
//////    


                reciteText3D.Material = correctColour;
                yield return new WaitForSeconds(CheckRecitedWordHandler.timeBetweenWordsInSeconds);
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
                AddScoreToScoreManager();
                MoveOnIfMoreWordsInList();
                break;
            case ECorrectResponseType.NEGATIVE_CORRECT_RESPONSE:
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Sad;
                reciteText3D.Material = incorrectColour;
                yield return new WaitForSeconds(CheckRecitedWordHandler.timeBetweenWordsInSeconds);
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
                RepeatSameWord();
                break;
            case ECorrectResponseType.UNKNOWN_CORRECT_RESPONSE:
                break;
            default:
                break;
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

    public void MoveOnIfMoreWordsInList ()
    {
        if (currentWordOrSentenceIndex < activeList.Count - 1)
        {
            GoToNextWordIfItExists();
        }

        else
        {
            // Mark the current list as complete, and move on to the next (if any) via changing booleans
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForEverything);
            if (activeList == currentWordOrSentenceList) { wordListComplete = true; }
            // No UI list in intro
            if (activeList == currentUiList || _levelManager.currentLevel == "Intro") { uiComplete = true; }
            if (_levelManager.currentLevel == "Level3") { openQuestionsComplete = true; }
            StartCoroutine(CheckWordListStatus());
        }
    }

    private IEnumerator CheckWordListStatus()
    {
        // Either proceed to next word list (ui), or end the game.
        // If level 1/2 have both lists done, or level 3 has open questions done, end the game.
        if (wordListComplete && uiComplete || openQuestionsComplete)
        {
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Finished!");
            GameOver();
        }
        else if (wordListComplete && !uiComplete)
        { 
            activeList = currentUiList;
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Great! Moving onto UI word list.");

            yield return new WaitForSeconds(2);
            StartCoroutine(StartCurrentWordCountdown());
            
        }

        else
        {
            Debug.LogError("Something went wrong in conditional for list changing.");
        }
    }

    public void GameOver()
    {
        reciteText3D.UpdateText("Say 'next' to proceed.\nOr 'repeat' to repeat.");
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForNextOrRepeat);
    }
}
