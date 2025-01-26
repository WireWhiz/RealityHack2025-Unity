using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;

public class TtsLogic : MonoBehaviour
{
    public GuessGameUI ui;
    private readonly List<Message> _msgs = new();

    private void Awake()
    {
        ui.OnTextInput += OnUsersInputHistory;
    }
    
    private void Start()
    {
        _msgs.Add(new Message(           
            Role.System,
            "You are video game character - Jammo." +
            "You are red robot that like to riddle user." +
            "Write your answer short and in one paragraph."));
    }
    
    private async void OnUsersInputHistory(string usrInput)
    {
        ui.ShowBotThinking();
        
        _msgs.Add(new Message(Role.User, usrInput));
        
        var api = new OpenAIClient(KeysStorage.Data.openai_key);
        var chatRequest = new ChatRequest(_msgs, Model.GPT4o);
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;
        _msgs.Add(response.FirstChoice.Message);
        
        ui.ShowOutputText(choice);

        var tts = GetComponent<ElevenLabsAPI>();
        StartCoroutine(tts.ConvertTextToSpeech(choice));
    }
}
