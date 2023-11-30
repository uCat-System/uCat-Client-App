using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MText;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Linq;
using Meta.WitAi;
using Meta.WitAi.TTS.Utilities;
using CandyCoded.env;
using EListeningState = WitListeningStateManager.ListeningState;

public class ConversationManager : MonoBehaviour
{
    
    
    // public Modular3DText _subtitle;
    // public Modular3DText _dialogue;
    public TTSSpeaker _uCatSpeaker;
    public TTSSpeaker _userSpeaker;

    private AudioSource uCatAudioSource;

    
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    private Coroutine submitEvery5s;

    private UIManager _uiManager;
    private WitListeningStateManager _witListeningStateManager;
    private LevelManager _levelManager;

    public bool isCurrentlyCountingTowardsTimeout;
    public float uCatResponseTimeoutLimit;
    public float uCatResponseTimeout;

    private AnimationDriver uCatAnimationDriver;
    public string standardInitializationMessage;
    public string advancedInitializationMessage;

    private Modular3DText subtitleText;


    void Start(){
        // Assign scripts
        _uiManager = GetComponent<UIManager>();
        _levelManager = GetComponent<LevelManager>();
        _witListeningStateManager = GetComponent<WitListeningStateManager>();
        subtitleText = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        uCatAnimationDriver = GameObject.FindWithTag("uCat").GetComponent<AnimationDriver>();

        standardInitializationMessage =  "Your name is 'uCat'. You are a humble, kind-hearted, compassionate, and sassy robocat. Sometimes you say \"meow\" when you speak. You help me learn how to use my implanted brain-computer interfaces to move inside the metaverse. You keep your responses short and to the point.";
        advancedInitializationMessage =  
        "Your name is 'uCat'. You are a humble, kind-hearted, compassionate, and sassy robocat. Sometimes you say \"meow\" when you speak. You help me learn how to use my implanted brain-computer interfaces to move inside the metaverse. You keep your responses short and to the point. At the end of each response, categorise your response into one of the following categories: 'happy' 'sad' 'confused' 'neutral' 'cheeky'. The category should be the last sentence of your response and just consist of the word by itself, e.g., 'Happy.'";
        if (_levelManager.currentLevel != "ConvoMode") {
            Debug.Log("ConversationManager.cs: Not in ConvoMode, returning");
            this.enabled = false;
            return;
        }


        // Initial dialogue and animation
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
        uCatResponseTimeout = 0;

        uCatAudioSource = GameObject.FindWithTag("uCatConversationAudioSource").GetComponent<AudioSource>();
        InitiliazeUcatConversation();
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForConversationModeInput);
        
        
    }

    public void HandlePartialSpeech(string text) {
        subtitleText.UpdateText(text);
    }

    public void HandleUserSpeech(string spokenText) {
        // This function is called from FreeSpeechManager when the user speaks (as long as they are allowed to currently)
        Debug.Log("ConversationManager.cs: HandleUserSpeech called with text: " + spokenText);
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Confused;

        _userSpeaker.Speak(spokenText);
        subtitleText.UpdateText(spokenText);
        GetOpenAIResponse(spokenText);
        _witListeningStateManager.TransitionToState(EListeningState.WaitingForConversationResponse);
    }
   
    private void InitiliazeUcatConversation(){
        if (env.TryParseEnvironmentVariable("OPENAI_API_KEY", out string apiKey))
            {
                api = new OpenAIAPI(apiKey);
                messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, advancedInitializationMessage)
                };
        }

        else {
            Debug.LogError("No API key found.");
        }
    }


    private async void GetOpenAIResponse(string textToSubmit){

        // Test commit change (safe)
        if( textToSubmit.Length < 1) return;

        // Construct the message object
        ChatMessage userMessage  = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = textToSubmit;
        if(userMessage.Content.Length > 100) userMessage.Content = userMessage.Content.Substring(0,100);

        //debugging
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        //add message to the list
        messages.Add(userMessage);

        //update textfield with user message
        // _dialogue.UpdateText(string.Format("You: {0}", userMessage.Content));

        //clear the input field
        // _subtitle.UpdateText("");

        //send entire chat to OpenAI to get its response
        isCurrentlyCountingTowardsTimeout = true;
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest(){
            Model = Model.ChatGPTTurbo,
            Temperature = 0.7,
            MaxTokens = 50,
            Messages = messages
        });
        
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;

        //get OpenAI response
        ChatMessage responseMessage = new ChatMessage();
        string response = chatResult.Choices[0].Message.Content;

        // This object will be added to the total messages
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = response;

        if (response.Length > 0) {
            string lastWord = GetLastWordOfLastSentence(response);
            // Get the normal sentence so uCat says it normally.
            string sentenceWithoutEmotion = response.Substring(0, response.Length - (lastWord.Length+1));

            // Play animation based on emotion catewgory
            PlayEmotionAnimation(lastWord);
            //TTS speak uCat's response
            _uCatSpeaker.Speak(sentenceWithoutEmotion);
            isCurrentlyCountingTowardsTimeout = false;
            uCatResponseTimeout = 0;
            //add the response to the total list of messages
            messages.Add(responseMessage);
        }

        else {
            // Catch it if API has an error somehow
            Debug.LogError("OpenAI API returned an empty response");
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForConversationModeInput);
        }


        //update the response text field
    }

    string GetLastWordOfLastSentence(string text)
    {
        // Split the text into sentences
        string[] sentences = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Get the last sentence
        string lastSentence = sentences.LastOrDefault()?.Trim();

        if (string.IsNullOrEmpty(lastSentence))
        {
            return string.Empty;
        }

        // Split the last sentence into words
        string[] words = lastSentence.Split(new char[] { ' ', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);

        // Return the last word
        return words.LastOrDefault();
    }

    void PlayEmotionAnimation(string text) {
        // Play the appropriate animation based on the emotion category

        Debug.Log("Emotion word: " + text);
        switch (text.ToLower()) {
            case "happy":
                uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
                break;
            case "sad":
                 uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Sad;
                break;
            case "confused":
                uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Confused;
                break;
            case "neutral":
                uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
                break;
            case "cheeky":
                uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Peeking;
                break;
            default:
                Debug.LogError("ExtractEmotionFromText: emotion not recognised");
                break;
        }

    }

    public void UcatIsDoneSpeaking() {
        Debug.Log("UcatIsDoneSpeaking called");
        _uCatSpeaker.Stop();
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForConversationModeInput);
    }

    void Update() {
        if (isCurrentlyCountingTowardsTimeout) {
            uCatResponseTimeout += Time.deltaTime;
        }

        if (uCatResponseTimeout > uCatResponseTimeoutLimit) {
                isCurrentlyCountingTowardsTimeout = false;
                uCatResponseTimeout = 0;
                UcatIsDoneSpeaking();
         }
    }
}