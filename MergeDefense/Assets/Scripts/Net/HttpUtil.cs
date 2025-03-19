using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpUtil : MonoBehaviour
{
    public static HttpUtil s_instance = null;

    void Awake()
    {
        s_instance = this;
    }

    public void reqGet(string url, Action<bool, string> callback = null)
    {
        StartCoroutine(GetRequest(url, callback));
    }

    public void reqPost(string url, string data, Action<bool, string> callback = null)
    {
        // Debug.Log(url);
        StartCoroutine(PostRequest(url, data, callback));
    }

    IEnumerator GetRequest(string url, Action<bool, string> callback = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(true, webRequest.downloadHandler.text);
                }
            }
            else
            {
                Debug.Log(webRequest.error + "\n" + webRequest.downloadHandler.text);
                if (callback != null)
                {
                    callback(false, webRequest.error);
                }
            }
        }
    }

    IEnumerator PostRequest(string url, string data, Action<bool, string> callback = null)
    {
        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            if (callback != null)
            {
                callback(true, webRequest.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log(webRequest.error + "\n" + webRequest.downloadHandler.text);
            
            if (callback != null)
            {
                callback(false, webRequest.error);
            }
        }
    }
}
