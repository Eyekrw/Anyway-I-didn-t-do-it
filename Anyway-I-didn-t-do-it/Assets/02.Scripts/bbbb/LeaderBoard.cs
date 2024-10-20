using SourGrape.bbbb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using TMPro;


namespace SourGrape.bbbb
{
    public class LeaderBoard : MonoBehaviour
    {
        public int idx = 0;
        public ScrollView scrollView;
        public GameObject Board;

        private string URL = Parameters.URL;
        private string path = "api/rank/read/";
        private float targetTime;
        private float updatePeriod = 30.0f;

        private List<GameObject> Cards;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("LB updater start");
            targetTime = Time.time;
            Cards = GetCards();
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > targetTime)
            {
                targetTime = Time.time + updatePeriod;
                StartCoroutine(GetRequest());
            }
        }


        public void BtnNext()
        {
            idx++;
            StartCoroutine(GetRequest());
        }

        public void BtnPrevious()
        {
            if (idx == 0)
            {
                idx = 0;
            }
            else
            {
                idx--;
            }
            StartCoroutine(GetRequest());
        }

        IEnumerator GetRequest()
        {
            //Debug.Log($"Get req start  URI: {URL + path + idx.ToString()} ");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(URL+path+idx.ToString()))
            {

                //webRequest.certificateHandler = new BypassCertificate();
                Debug.Log($"idx : {idx}  Code : {webRequest.responseCode}");
                webRequest.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("JWT")}");

                yield return webRequest.SendWebRequest();
                if(webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    string ret = webRequest.downloadHandler.text;
                    try
                    {
                        List<PlayerRankInfo> list = JsonConvert.DeserializeObject<List<PlayerRankInfo>>(ret);
                        for (int i = 1; i <= 10; i++)
                        {
                            if (list.Count >= i)
                            {
                                //Cards[i - 1].GetComponentsInChildren<TMP_Text>()[0].text = $"#{idx * 10 + i} {list[i - 1].nickname.ToString()}";
                                Cards[i - 1].GetComponentsInChildren<TMP_Text>()[1].text = $"{list[i - 1].nickname.ToString()}";
                                Cards[i - 1].GetComponentsInChildren<TMP_Text>()[0].text = $"점수: {list[i - 1].winCnt.ToString()}";
                            }
                            else
                            {
                                Cards[i - 1].GetComponentsInChildren<TMP_Text>()[0].text = "";
                                Cards[i - 1].GetComponentsInChildren<TMP_Text>()[1].text = "";
                            }
                        }
                    }
                    catch
                    {
                        idx--; idx = idx < 0 ? idx : 0;
                    }
                }
            }
        }

        private List<GameObject> GetCards()
        {
            List<GameObject> ret = new List<GameObject>();

            foreach (Transform t in Board.transform)
            {
                ret.Add(t.gameObject);
            }
            return ret;
        }
    }

    [System.Serializable]
    public class PlayerRankInfo
    {
        public string id;
        public int winCnt;
        public string nickname;
        public string message;
        public int statusCode;
    }
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // 모든 인증서가 유효하다고 간주
            return true;
        }
    }
}
