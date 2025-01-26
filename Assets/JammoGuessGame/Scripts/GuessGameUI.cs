using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuessGameUI : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text output;
    public Button micButton;
    public Image micButtonImage;
    public Image outputImage;

    public Sprite micButtonReady;
    public Sprite micButtonRecording;

    public RectTransform outputScrollRect;
    public RectTransform outputImageRect;

    private bool _isRecordingAudio;
    private AudioClip _clip;

    public event System.Action OnAudioStarted;
    public event System.Action<string> OnTextInput;
    public event System.Action OnAudioSubmitted; 

    private void Awake()
    {
        micButton.onClick.AddListener(OnMicButtonClicked);
        input.onSubmit.AddListener(OnSubmitEvent);
    }

    public void OnMicButtonClicked()
    {
        _isRecordingAudio = !_isRecordingAudio;
        if (_isRecordingAudio)
        {
            if(micButtonImage)
                micButtonImage.sprite = micButtonRecording;
            OnAudioStarted?.Invoke();
        }
        else
        {
            if (micButtonImage)
                micButtonImage.sprite = micButtonReady;
            OnAudioSubmitted?.Invoke();
        }
    }
    
    private void OnSubmitEvent(string usrInput)
    {
        OnTextInput?.Invoke(usrInput);
    }

    public void ShowOutputText(string outputStr)
    {
        output.text = outputStr;
        ShowOutput(true);
    }

    public void ShowBotThinking()
    {
        output.text = "...";
        ShowOutput(true);
    }

    public void ShowInputText(string usrInput)
    {
        //input.text = usrInput;
        OnSubmitEvent(usrInput);
    }

    public void ShowImage(Sprite image)
    {
        ShowOutput(false);
        if (outputImage)
        {
            outputImage.sprite = image;
        }
    }

    private void ShowOutput(bool showText)
    {
        //outputScrollRect.gameObject.SetActive(showText);
        //outputImageRect.gameObject.SetActive(!showText);
    }
}
