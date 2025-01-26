using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;
using Whisper;
using Whisper.Utils;

public class AsrLogic : MonoBehaviour
{
    public ImageGenAI genAi;
    public WhisperManager whisper;
    public MicrophoneRecord microphone;
    public Cubesat cubesat;
    public Transform parkPos;
    private readonly List<Message> _msgs = new();


    private bool _isRecordingAudio;
    private Cubesat.DialogPath DialogPath;
    private Cubesat.DialogStep DialogStep;


    private void Awake()
    {
        genAi.OnImageGenerated += OnImageGenerate;
        microphone.OnRecordStop += OnUsersAudio;
        cubesat = Object.FindAnyObjectByType<Cubesat>();
    }

    public void DisplayDialog(string dialog)
    {
        DialogStep.dialog = dialog;
        cubesat.StartDialog(DialogPath.pathName);
    }


    public void OnMicButtonClicked()
    {
        _isRecordingAudio = !_isRecordingAudio;
        if (_isRecordingAudio)
        {
            microphone.StartRecord();
        }
        else
        {
            microphone.StopRecord();
        }
    }


    private async void OnUsersAudio(AudioChunk audio)
    {
        DisplayDialog("Computing...");
        var res = await whisper.GetTextAsync(audio.Data, audio.Frequency, audio.Channels);
        OnUsersInputTools(res.Result);
    }

    private void Start()
    {
       _msgs.Add(new Message(           
           Role.System,
           "You are a robotic astronaut assistant - Cutesat." +
           "You are cute robot that is tasked with helping the player, the astronaut, with any tasks it needs like repairing the spaceship."));

        DialogPath = new Cubesat.DialogPath();
        DialogPath.pathName = "ai output";

        DialogStep = new Cubesat.DialogStep();
        DialogStep.parkPos = parkPos;
        DialogStep.lookTarget = Camera.main.transform;
        DialogStep.charactersPerSecond = 40;

        DialogPath.steps = new()
        {
            DialogStep
        };

        cubesat.dialogPaths.Add(DialogPath);
    }

    private async void OnUsersInputTools(string usrInput)
    {
        DisplayDialog("Thinking...");
        
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

        DisplayDialog(choice);
        
        //var tts = GetComponent<ElevenLabsAPI>();
        //StartCoroutine(tts.ConvertTextToSpeech(choice));
    }
    
    private void OnImageGenerate(Texture2D tex)
    {
        var sprite = Sprite.Create(tex, 
            new Rect(0.0f, 0.0f, tex.width, tex.height), 
            new Vector2(0.5f, 0.5f), 100.0f
        );
        Debug.LogWarning("I turned off texture gen cause I didn't want to reimplement whatever UI");
        //ui.ShowImage(sprite);
    }
}
