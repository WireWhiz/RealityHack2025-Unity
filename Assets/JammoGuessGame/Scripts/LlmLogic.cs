using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;

public class LlmLogic : MonoBehaviour
{
    public GuessGameUI ui;

    private void Awake()
    {
        ui.OnTextInput += OnUsersInputHistory; //OnUsersInput
    }

    private async void OnUsersInput(string usrInput)
    {
        ui.ShowBotThinking();
        
        var api = new OpenAIClient(KeysStorage.Data.openai_key);
        var messages = new List<Message>
        {
            new Message(Role.User, usrInput),
        };
        var chatRequest = new ChatRequest(messages, Model.GPT4o);
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;
        
        ui.ShowOutputText(choice);
    }

    private readonly List<Message> _msgs = new();

    private void Start()
    {
       _msgs.Add(new Message(           
           Role.System,
           "You are a robotic astronaut assistant - Cutesat." +
           "You are cute robot that is tasked with helping the player, the astronaut, with any tasks it needs like repairing the spaceship."));
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
    }

    private async void OnUsersInputTools(string usrInput)
    {
        ui.ShowBotThinking();
        
        _msgs.Add(new Message(Role.User, usrInput));
        
        var api = new OpenAIClient(KeysStorage.Data.openai_key);
        
        var tools = new List<Tool>
        {
            Tool.FromFunc<bool>(
                "generate_sd_image", () => { ui.ShowImage(null); return true;}, 
                "Generate image using Stable Diffusion with provided prompt"
            )
        };
        
        var chatRequest = new ChatRequest(_msgs, tools,  "auto",Model.GPT4o);
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;
        _msgs.Add(response.FirstChoice.Message);

        var toolCalls = response.FirstChoice.Message.ToolCalls;
        if (toolCalls != null)
        {
            foreach (var toolCall in response.FirstChoice.Message.ToolCalls)
            {
                var functionResult = await toolCall.InvokeFunctionAsync();
                _msgs.Add(new Message(toolCall, functionResult));
            }
            return;
        }

        ui.ShowOutputText(choice);
    }
}
