using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

public class ImageGenAI : MonoBehaviour
{
    [Serializable]
    public class RequestGenAi
    {
        public string prompt;
    }

    public event Action<Texture2D> OnImageGenerated;

    public IEnumerator GenerateImage(string imgPrompt)
    {
        // Prepare the API URL
        string url = "https://5000-01jj2w2j6436xrj9c2p71hzjs2.cloudspaces.litng.ai/generate";

        // Prepare the request body
        string jsonBody = JsonUtility.ToJson(new RequestGenAi {prompt=imgPrompt});

        // Create a UnityWebRequest with POST method
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] imgData = request.downloadHandler.data;
            var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.LoadImage(imgData);
            OnImageGenerated?.Invoke(texture);
        }
    }
}
