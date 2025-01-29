using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using TMPro;
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
    public string modelName = "deepseek-r1:14b";


    private bool _isRecordingAudio;
    private Cubesat.DialogPath DialogPath;
    private Cubesat.DialogStep DialogStep;

    private bool isThinking = false;
    public TextMeshProUGUI thinkingDebug;


    private void Awake()
    {
        genAi.OnImageGenerated += OnImageGenerate;
        microphone.OnRecordStop += OnUsersAudio;
        cubesat = Object.FindAnyObjectByType<Cubesat>();
    }

    bool appendingDialog = false;
    public void DisplayDialog(string dialog)
    {
        DialogStep.dialog = dialog;
        cubesat.StartDialog(DialogPath.pathName);
        appendingDialog = false;
    }

    public void AppendDialog(string dialog)
    {
        if(dialog == "<think>")
        {
            isThinking = true;
            return;
        }
        else if (dialog == "</think>")
        {
            isThinking &= !isThinking;
            return;
        }

        if (!isThinking)
        {
            if (appendingDialog)
                cubesat.AppendDialog(dialog);
            else
                DisplayDialog(dialog);
            appendingDialog = true;
        }
        else
        {
            if (thinkingDebug)
            {

                if (dialog.Contains("\n"))
                {
                    // I could care less about this project so I'm doing this dirty
                    var lines = (thinkingDebug.text + dialog).Split('\n');
                    thinkingDebug.text = "";
                    for (int i = Mathf.Max(0, lines.Length - 7); i < lines.Length; i++)
                    {
                        thinkingDebug.text += lines[i] + '\n';
                    }
                }
                else
                {
                    thinkingDebug.text += dialog;
                }
            }
        }
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
        DisplayDialog("Transcribing...");
        var res = await whisper.GetTextAsync(audio.Data, audio.Frequency, audio.Channels);
        OnUsersInputTools(res.Result);
    }

    private async void Start()
    {
       string context = "You are a robotic astronaut assistant - Cutesat." +
           "You are cute robot that is tasked with helping the player, the astronaut, with any tasks it needs like repairing the spaceship." +
           "You are aware the spaceship's Environmental Control and Life Support Syste (ECLSS) has power fluctuations." +
            "And, there is Thermal regulation failure in Sector 4." +
            "And, the Communication antenna is misaligned." +
            "Your mission is to assist the astronaut with maintaining the spaceship." +
            "The astronaut's mission is to explore deep space." +
            "You know that the astronaut is traveling alone and is far away from Earth and experiences social isolation because of that." +
            "Your ultimate goal is to simulate a human connection for the astronaut, and you can explain your goal to the astronaut." +
            "Keep your responses under 35 words, ideally less that that.";

        Ollama.InitChat();

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

        await Ollama.AppendData(context);
    }

    private async void OnUsersInputTools(string usrInput)
    {
        DisplayDialog("Thinking...");

        await Ollama.ChatStream((string text) => { 
            AppendDialog(text);
        }, modelName, usrInput);
        
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
