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
public class ConversationManager : MonoBehaviour
{
    
    
    // public Modular3DText _subtitle;
    // public Modular3DText _dialogue;
    // public TTSSpeaker _uCatSpeaker;
    // public TTSSpeaker _userSpeaker;

    
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    private Coroutine submitEvery5s;

    private UIManager _uiManager;
    private LevelManager _levelManager;



    void Start(){

        _uiManager = GetComponent<UIManager>();
        _levelManager = GetComponent<LevelManager>();
        if (_levelManager.currentLevel != "ConvoMode") {
            Debug.Log("ConversationManager.cs: Not in ConvoMode, returning");
            this.enabled = false;
            return;
        }
        
        // api = new OpenAIAPI("");
        // StartConversation();
        // submitEvery5s = StartCoroutine(SubmitToOpenAI(5));
        
    }

    // IEnumerator SubmitToOpenAI(int t){
        // int count = 1;

        // while(count < 6){
        //     yield return new WaitForSeconds(t);

        //     if (_subtitle.Text.Length > 1){
        //         //TTS speak user's input
        //         _userSpeaker.Speak(_subtitle.Text);
        //         GetOpenAIResponse();
                
        //         /*
        //         //simulating whether my coroutine would submit new voice decoding to OpenAI every couple s      
        //         dialogue.UpdateText(string.Format("You: {0}", subtitle.Text));
        //         subtitle.UpdateText("");
        //         */
        //         Debug.Log(string.Format("#{0} : OpenAI submission", count));
        //         count++;
        //     }
        // }
    // }
   
    private void StartConversation(){
        // messages = new List<ChatMessage>{
        //     new ChatMessage(ChatMessageRole.System, "Your name is 'uCat'. You are a humble, kind-hearted, compassionate, and sassy robocat. Sometimes you say \"meow\" when you speak. You help me learn how to use my implanted brain-computer interfaces to move inside the metaverse. You keep your responses short and to the point.")
        // };
    }


    private async void GetOpenAIResponse(){
    //     if(_subtitle.Text.Length < 1)return;

    //     //access the input and send to OpenAI
    //     ChatMessage userMessage  = new ChatMessage();
    //     userMessage.Role = ChatMessageRole.User;
    //     userMessage.Content = _subtitle.Text;
    //     if(userMessage.Content.Length > 100) userMessage.Content = userMessage.Content.Substring(0,100);

    //     //debugging
    //     Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

    //     //add message to the list
    //     messages.Add(userMessage);

    //     //update textfield with user message
    //     _dialogue.UpdateText(string.Format("You: {0}", userMessage.Content));

    //     //clear the input field
    //     _subtitle.UpdateText("");

    //     //send entire chat to OpenAI to get its response
    //     var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest(){
    //         Model = Model.ChatGPTTurbo,
    //         Temperature = 0.7,
    //         MaxTokens = 50,
    //         Messages = messages
    //     });

    //     //get OpenAI response
    //     ChatMessage responseMessage = new ChatMessage();
    //     responseMessage.Role = chatResult.Choices[0].Message.Role;
    //     responseMessage.Content = chatResult.Choices[0].Message.Content;
    //     //debugging
    //     Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

    //     //TTS speak uCat's response
    //     _uCatSpeaker.Speak(responseMessage.Content);

    //     //add the response to the total list of messages
    //     messages.Add(responseMessage);

    //     //update the response text field
    //     _dialogue.UpdateText(string.Format("You: {0}\n\nuCat: {1}", userMessage.Content, responseMessage.Content));
    }
}