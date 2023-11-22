using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MText;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
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



    void Start(){

        _uCatSpeaker.Speak("Meow, I'm uCat. Let's have a conversation!");
        uCatAnimationDriver = GameObject.FindWithTag("uCat").GetComponent<AnimationDriver>();
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
        uCatResponseTimeout = 0;
        _uiManager = GetComponent<UIManager>();
        _levelManager = GetComponent<LevelManager>();
        _witListeningStateManager = GetComponent<WitListeningStateManager>();

        if (_levelManager.currentLevel != "ConvoMode") {
            Debug.Log("ConversationManager.cs: Not in ConvoMode, returning");
            this.enabled = false;
            return;
        }
        uCatAudioSource = GameObject.FindWithTag("uCatConversationAudioSource").GetComponent<AudioSource>();
        InitiliazeUcatConversation();
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForConversationModeInput);
        
        
    }

    public void HandleUserSpeech(string spokenText) {
        // This function is called from FreeSpeechManager when the user speaks (as long as they are allowed to currently)
        Debug.Log("ConversationManager.cs: HandleUserSpeech called with text: " + spokenText);
        uCatAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Confused;

        _userSpeaker.Speak(spokenText);
        GetOpenAIResponse(spokenText);
        _witListeningStateManager.TransitionToState(EListeningState.WaitingForConversationResponse);
    }
   
    private void InitiliazeUcatConversation(){
        if (env.TryParseEnvironmentVariable("OPENAI_API_KEY", out string apiKey))
            {
                api = new OpenAIAPI(apiKey);
                messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Your name is 'uCat'. You are a humble, kind-hearted, compassionate, and sassy robocat. Sometimes you say \"meow\" when you speak. You help me learn how to use my implanted brain-computer interfaces to move inside the metaverse. You keep your responses short and to the point.")
                };
        }

        else {
            Debug.LogError("No API key found.");
        }
    }


    private async void GetOpenAIResponse(string textToSubmit){

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
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        //debugging
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        if (responseMessage.Content.Length > 0) {
            //TTS speak uCat's response
            _uCatSpeaker.Speak(responseMessage.Content);
            isCurrentlyCountingTowardsTimeout = false;
            uCatResponseTimeout = 0;
            //add the response to the total list of messages
            messages.Add(responseMessage);
            // _dialogue.UpdateText(string.Format("You: {0}\n\nuCat: {1}", userMessage.Content, responseMessage.Content));
        }

        else {
            // Catch it if API has an error somehow
            Debug.LogError("OpenAI API returned an empty response");
            Debug.Log(responseMessage.Content);
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForConversationModeInput);
        }


        //update the response text field
    }

    public void UcatIsDoneSpeaking() {
        Debug.Log("UcatIsDoneSpeaking called");
        _uCatSpeaker.Stop();
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