using System.Threading.Tasks;
using UnityEngine;

public class DummyLogic : MonoBehaviour
{
    public GuessGameUI ui;

    private void Awake()
    {
        ui.OnTextInput += OnUsersInput;
        ui.OnAudioSubmitted += OnUsersAudio;
    }

    private async void OnUsersInput(string usrInput)
    {
        ui.ShowBotThinking();
        
        // TODO: generate smart answer
        await Task.Delay(1000);
        if (usrInput != "image")
            ui.ShowOutputText("I'm sorry Dave, I'm afraid I can't do that!");
        else
            ui.ShowImage(null);
    }
    
    private async void OnUsersAudio()
    {
        // TODO: actually get users answer
        await Task.Delay(1000);
        var input = "Test input";
        
        ui.ShowInputText(input);
        OnUsersInput(input);
    }
}
