using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;

public class ImageLogic : MonoBehaviour
{
    public GuessGameUI ui;
    public ImageGenAI genAi;
    private readonly List<Message> _msgs = new();

    private void Awake()
    {
        ui.OnTextInput += OnUsersInputTools;
        genAi.OnImageGenerated += OnImageGenerate;
    }

    private void Start()
    {
       _msgs.Add(new Message(           
           Role.System,
           "You are video game character - Jammo." +
           "You are red robot that like to riddle user." +
           "You can riddle some image and ask user to guess what you drew." +
           "Draw something simple, singular well-known object."));
    }

    private async void OnUsersInputTools(string usrInput)
    {
        ui.ShowBotThinking();
        
        _msgs.Add(new Message(Role.User, usrInput));
        
        var api = new OpenAIClient(KeysStorage.Data.openai_key);
        
        var tools = new List<Tool>
        {
            Tool.FromFunc<string, bool>(
                "generate_sd_image", (prompt) =>
                {
                    StartCoroutine(genAi.GenerateImage(prompt));
                    return true;
                }, 
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
        
        var tts = GetComponent<ElevenLabsAPI>();
        StartCoroutine(tts.ConvertTextToSpeech(choice));
    }
    
    private void OnImageGenerate(Texture2D tex)
    {
        var sprite = Sprite.Create(tex, 
            new Rect(0.0f, 0.0f, tex.width, tex.height), 
            new Vector2(0.5f, 0.5f), 100.0f
        );
        ui.ShowImage(sprite);
    }
}
