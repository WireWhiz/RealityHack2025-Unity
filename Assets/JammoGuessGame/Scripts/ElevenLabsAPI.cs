using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenLabsAPI : MonoBehaviour
{
    private static string apiKey = KeysStorage.Data.elevenlabs;
    private const string voiceId = "9BWtsMINqrJLrRacOk9x";    // Replace with your voice ID

    [Serializable]
    private class Request
    {
        public string text;
    }
    
    public IEnumerator ConvertTextToSpeech(string textToConvert)
    {
        // Prepare the API URL
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}";

        // Prepare the request body
        string jsonBody = JsonUtility.ToJson(new Request {text=textToConvert});

        // Create a UnityWebRequest with POST method
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the headers
        request.SetRequestHeader("sk_8a7d0eca72e60567339e80ea7821ea65002d0ce3bb295fea", apiKey); //xi-api-key
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TTS Request successful!");

            // Get the returned MP3 data
            byte[] mp3Data = request.downloadHandler.data;

            // Convert the MP3 data into an AudioClip
            yield return ConvertMp3ToAudioClip(mp3Data);
        }
        else
        {
            Debug.LogError($"TTS Request failed: {request.error}");
        }
    }

    private IEnumerator ConvertMp3ToAudioClip(byte[] mp3Data)
    {
        // Save MP3 data to a temporary file
        string tempFilePath = Application.persistentDataPath + "/tempAudio.mp3";
        System.IO.File.WriteAllBytes(tempFilePath, mp3Data);

        // Load the MP3 file as an AudioClip
        using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.MPEG))
        {
            yield return audioRequest.SendWebRequest();

            if (audioRequest.result == UnityWebRequest.Result.Success)
            {
                // Get the AudioClip
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                Debug.Log("AudioClip successfully created!");

                // Play the AudioClip (or do whatever you want with it)
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to convert MP3 to AudioClip: {audioRequest.error}");
            }
        }

        // Optionally delete the temporary file after usage
        System.IO.File.Delete(tempFilePath);
    }
}
