using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SourGrape.kiyoung
{
    public class HttpRequestManager : MonoBehaviour
    {
        public IEnumerator PostRequest(string url, string jsonData, System.Action<UnityWebRequest> callback)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            callback(request);
        }
    }
}
