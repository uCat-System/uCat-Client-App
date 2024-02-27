using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MText;
using EListeningState = WitListeningStateManager.ListeningState;
using ECorrectResponseType = CheckRecitedWordHandler.CorrectResponseType;

public class WordReciteManager : MonoBehaviour
{    
    private AnimationDriver catAnimationDriver;

    private Modular3DText dialogueText3D;
    
    // Current word tracking
    public int currentWordOrSentenceIndex;

    // Word lists
    private List<string> currentWordOrSentenceList;

    private List<string> currentUiList;

    // This is the list that is active, either words/sentences OR UI list
    public List<string> activeList;

    // Scriptable asset for externally stored word lists and audio
    public WordLists wordLists;

    private AudioClip[] uCatOpenQuestionAudioList;

    // Track if the lists have been completed
    bool wordListComplete;
    
    bool uiComplete;

    bool openQuestionsComplete;

    int incorrectWordAttempts;

    // Text colours

    public Material correctColour;
    public Material incorrectColour;
    public Material defaultColour;
    public Material listeningColour;

    // External Managers

    private FreeSpeechManager _freeSpeechManager;

    private  WitListeningStateManager _witListeningStateManager;
    private UIManager _uiManager;
    private LevelManager _levelManager;
    private DialogueManager _dialogueManager;

    // 3D Text 

    private Modular3DText reciteText3D;
    private Modular3DText partialText3D;

    private Modular3DText subtitleText3D;

    // Audio

    private AudioSource reciteBoardAudioSource;
    private AudioSource uCatAudioSource;

    public AudioClip[] wordSounds;
    public void BeginReciteTask() {
        if (_levelManager.CurrentLevel == "Level3") {
            BeginFreestyleTask();
        } else {
            StartCoroutine(StartCurrentWordCountdown());
        }
    }

    public void BeginFreestyleTask() {
        // Freestyle (open questions) task
        // Words come up instantly and take up the whole board (alternate board)
        // - play audio along with them (tbd)
        dialogueText3D.UpdateText("");
        subtitleText3D.UpdateText("");
        reciteText3D.UpdateText("");

        reciteBoardAudioSource.clip = wordSounds[0]; // replace this with ucat voice later
        reciteBoardAudioSource.Play();

        subtitleText3D.UpdateText("");

        string question = activeList[currentWordOrSentenceIndex]; 
        uCatAudioSource.PlayOneShot(uCatOpenQuestionAudioList[currentWordOrSentenceIndex]);

        dialogueText3D.UpdateText(question);
        reciteText3D.Material = defaultColour;
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForFreestyleResponse);
    }
    void Start()
    {
        // Assigning gameobjects
        uCatAudioSource = GameObject.FindWithTag("uCat").GetComponent<AudioSource>();
        catAnimationDriver = GameObject.FindWithTag("uCat").GetComponent<AnimationDriver>();
        subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
        reciteText3D = GameObject.FindWithTag("ReciteText3D").GetComponent<Modular3DText>();
        reciteBoardAudioSource = GameObject.FindWithTag("ReciteBoard").GetComponent<AudioSource>();
        dialogueText3D = GameObject.FindWithTag("DialogueText3D").GetComponent<Modular3DText>();

        // Managers

        _uiManager = GetComponent<UIManager>();
        _freeSpeechManager = GetComponent<FreeSpeechManager>();
        _witListeningStateManager = GetComponent<WitListeningStateManager>();
        _levelManager = GetComponent<LevelManager>();
        _dialogueManager = GetComponent<DialogueManager>();

       
        // Game state variables
        uiComplete = false;
        wordListComplete = false;

        // Initialise Word Lists
        SetWordAndUiListsBasedOnLevel();
        activeList = currentWordOrSentenceList;
        currentWordOrSentenceIndex = 0;
        incorrectWordAttempts = 0;

        // Start the first word
        reciteText3D.Material = defaultColour;
    }

    void SetWordAndUiListsBasedOnLevel() {
        switch (_levelManager.CurrentLevel) {
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
                
                // Initialise word audio (for open questions)
                uCatOpenQuestionAudioList = new AudioClip[currentWordOrSentenceList.Count];
                uCatOpenQuestionAudioList = wordLists.level3OpenQuestionsAudioList.ToArray();
                break;
        }
    }


    public IEnumerator StartCurrentWordCountdown()
    {
        dialogueText3D.UpdateText("");
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
        StartCoroutine(ChangeTimeOutText());
    }

    public void GoToNextWordIfItExists()
    {
        // If the next word does not exceed the limit
        if (currentWordOrSentenceIndex < activeList.Count-1)
        {
            currentWordOrSentenceIndex++;
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
            BeginReciteTask();
        } else {
            // End of list
            LevelTaskIsComplete();
        }
    }

    public void RepeatSameWord()
    {
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
         BeginReciteTask();
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
        } else {
            // Compare the uttered text with the correct text
            ECorrectResponseType correctResponseType = CheckRecitedWordHandler.CheckIfWordOrSentenceIsCorrect(text, activeList[currentWordOrSentenceIndex]);
            StartCoroutine(HandleWordOrSentenceCorrectOrIncorrect(correctResponseType));
        }

    }
    
    public IEnumerator HandleWordOrSentenceCorrectOrIncorrect(ECorrectResponseType responseType) {
        switch (responseType) {

            // If the word was right
            case ECorrectResponseType.POSITIVE_CORRECT_RESPONSE:
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
                reciteText3D.Material = correctColour;
                yield return new WaitForSeconds(CheckRecitedWordHandler.timeBetweenWordsInSeconds);
                dialogueText3D.UpdateText("");
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
                MoveOnIfMoreWordsInList();
                break;

            // If the word was wrong

            case ECorrectResponseType.NEGATIVE_CORRECT_RESPONSE:
                PlayIncorrectWordDialogue();
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Sad;
                reciteText3D.Material = incorrectColour;

                // Wait for the amount of seconds that the audio clip goes for, so we don't overlap

                if (uCatAudioSource.isPlaying) {
                    yield return new WaitWhile(() => uCatAudioSource.isPlaying);
                } else {
                    yield return new WaitForSeconds(CheckRecitedWordHandler.timeBetweenWordsInSeconds);
                }

                dialogueText3D.UpdateText("");
                catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
                RepeatSameWord();
                break;
            case ECorrectResponseType.UNKNOWN_CORRECT_RESPONSE:
                break;
            default:
                break;
        }
    }

    void PlayIncorrectWordDialogue() {
        string incorrectResponseText; 
        if (incorrectWordAttempts <= 2) {
            incorrectResponseText = CheckRecitedWordHandler.negativeCorrectResponses[incorrectWordAttempts];
            uCatAudioSource.PlayOneShot(CheckRecitedWordHandler.negativeCorrectResponseAudio[incorrectWordAttempts]);
        } else {
            incorrectResponseText = CheckRecitedWordHandler.negativeCorrectResponses[3];
        }

        dialogueText3D.UpdateText(incorrectResponseText);
        incorrectWordAttempts++;
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
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
            if (activeList == currentWordOrSentenceList) { wordListComplete = true; }
            // No UI list in intro
            if (activeList == currentUiList || _levelManager.CurrentLevel == "Intro") { uiComplete = true; }
            if (_levelManager.CurrentLevel == "Level3") { 
                openQuestionsComplete = true;
                LevelTaskIsComplete();
             }
            StartCoroutine(CheckWordListStatus());
        }
    }

    private IEnumerator CheckWordListStatus()
    {
        // Either proceed to next word list (ui), or end the game.
        if (wordListComplete && uiComplete || openQuestionsComplete)
        {
            currentWordOrSentenceIndex = 0;
            LevelTaskIsComplete();
        }
        else if (wordListComplete && !uiComplete)
        { 
            activeList = currentUiList;
            currentWordOrSentenceIndex = 0;
            reciteText3D.UpdateText("Great! Moving onto UI word list.");
            yield return new WaitForSeconds(2);
            BeginReciteTask();    
        }

        else
        {
            Debug.LogError("Something went wrong in conditional for list changing.");
        }
    }

    public void LevelTaskIsComplete()
    {
        StopAllCoroutines();
        // This ensures the timer will stop counting (once levels done)
        _freeSpeechManager.OnStoppedListening();
        _uiManager.ShowOrHideReciteMesh(false);
        reciteText3D.UpdateText("");
        _dialogueManager.StartDialogue();
    }
}
