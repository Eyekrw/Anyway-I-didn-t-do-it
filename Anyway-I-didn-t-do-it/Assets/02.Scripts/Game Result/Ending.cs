using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SocialPlatforms.Impl;
using System;
using UnityEngine.SceneManagement;
using SourGrape.bbbb;
using SourGrape.kiyoung;
using UnityEngine.Networking;
using UnityEngine.Windows;
using Newtonsoft.Json;
using SourGrape.bbbb;
using System.IO;
using System.Text;
using System.Net;
using Photon.Pun;
using Photon.Realtime;
using System.Security.Cryptography;
using System.Globalization;

namespace SourGrape.yewon
{
    public class Ending : MonoBehaviourPunCallbacks
    {
        public GameObject MainCamera;
        public TextMeshProUGUI WinnerT;

        private bool _moveCamera = false;
        [SerializeField]
        private float _rotationSpeed = 10f;
        private Quaternion _targetRotation;
        private Color _winnerC;
        [SerializeField]
        private float _fadeSpeed = 0.17f;
        private GameObject Back2MainBtn;

        private List<int> topScorers;
        private HttpRequestManager _httpRequestManager;

        void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            MainCamera = GameObject.Find("Main Camera");
            MainCamera.transform.rotation = Quaternion.Euler(new Vector3(-90, 180, 0));
            WinnerT = GameObject.Find("Winner").GetComponent<TextMeshProUGUI>();
            _winnerC = WinnerT.color;
            StartCoroutine(ShowEnding());
            _targetRotation = MainCamera.transform.rotation;
            Back2MainBtn = GameObject.Find("BackBtn");
            Back2MainBtn.SetActive(false);
            _httpRequestManager = new HttpRequestManager();

            // 점수가 가장 높은 플레이어 확인
            topScorers = new List<int>();
            int highestScore = int.MinValue;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                int actNum = player.ActorNumber;
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Score" + actNum))
                {
                    Debug.Log(player.NickName + " " + actNum + " " + PhotonNetwork.CurrentRoom.CustomProperties["Score" + actNum]);

                    int score = (int)PhotonNetwork.CurrentRoom.CustomProperties["Score" + actNum];
                    // update highest score and top scorers
                    if (score > highestScore)
                    {
                        highestScore = score;
                        topScorers.Clear();
                        topScorers.Add(actNum);
                    }
                    else if (score == highestScore)
                    {
                        topScorers.Add(actNum);
                    }
                }
            }

            // create random object
            System.Random random = new System.Random();

            // spawn positions
            List<Vector3> spawnPositions = new List<Vector3>
            {
                new Vector3(4.03f, 14.16f, 0.39f),
                new Vector3(0.43f, 13.86f, 3.61f),
                new Vector3(-2.69f, 15.76f, -1.27f),
                new Vector3(-3.48f, 13.55f, 3.60f)
            };

            // instantiate player in each client
            foreach (int actNum in topScorers)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Character" + actNum))
                {
                    string characterName = (string)PhotonNetwork.CurrentRoom.CustomProperties["Character" + actNum];

                    // select random spawn position
                    int index = random.Next(spawnPositions.Count);
                    Vector3 spawnPosition = spawnPositions[index];
                    spawnPositions.RemoveAt(index); // 사용한 위치는 리스트에서 제거

                    // calculate direction to look at MainCamera's x, z position
                    Vector3 direction = new Vector3(MainCamera.transform.position.x, spawnPosition.y, MainCamera.transform.position.z) - spawnPosition;
                    Quaternion rotation = Quaternion.LookRotation(direction);

                    // Instantiate locally
                    GameObject characterPrefab = Resources.Load<GameObject>("bbbb/Characters/" + characterName);
                    if (characterPrefab != null)
                    {
                        GameObject obj = Instantiate(characterPrefab, spawnPosition, rotation);
                        string prefabName = obj.name;
                        string characterType = "";
                        for (int k = 0; k < prefabName.Length; k++)
                        {
                            if (prefabName[k] == ' ' || prefabName[k] == '\0')
                                break;
                            characterType += prefabName[k];
                        }
                        string animationName = characterType + "_Dance";
                        obj.transform.GetComponent<Animator>().Play(animationName);
                    }
                    else
                    {
                        Debug.LogError("Character prefab not found: " + characterName);
                    }
                }
                else
                {
                    Debug.LogError("Characters" + actNum + " is not in the room's custom properties.");
                }
            }
            StartCoroutine(ShowBtn());
        }

        void Update()
        {
            if (_moveCamera)
            {
                if (Time.time % 1 < Time.deltaTime)
                {
                    _rotationSpeed *= 1.2f;
                }
                _targetRotation *= Quaternion.Euler(_rotationSpeed * Time.deltaTime, 0, 0); // X축으로 1도씩 회전
                MainCamera.transform.rotation = Quaternion.Slerp(MainCamera.transform.rotation, _targetRotation, Time.deltaTime);
                _winnerC.a -= Time.deltaTime * _fadeSpeed;
                WinnerT.color = _winnerC;
                if (MainCamera.transform.eulerAngles.x >= 7 && MainCamera.transform.eulerAngles.x <= 10)
                {
                    _moveCamera = false;
                }
            }
        }

        private IEnumerator ShowEnding()
        {
            yield return new WaitForSeconds(1f);
            _moveCamera = true;
        }

        private IEnumerator ShowBtn()
        {
            yield return new WaitForSeconds(10f);
            Back2MainBtn.SetActive(true);
        }

        public void Back2Main()
        {
            bool isCustom = false;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Custom"))
            {
                isCustom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["Custom"];
            }
            if (!isCustom)
            {
                string url = Parameters.URL + "api/rank/win";
                for (int i = 0; i < topScorers.Count; i++)
                {
                    if (topScorers[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        string nickname = PhotonNetwork.NickName;
                        string jsonString = $"{{ \"nickname\": \"{nickname}\" }}";
                        StartCoroutine(UpdateRank(url, jsonString));
                        //SceneManager.LoadScene("LobbyScene_bbbb");
                        break;
                    }
                }
            }
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }
        private IEnumerator UpdateRank(string url, string jsonString)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("JWT")}");

            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("good" + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("fail" + request.error);
            }
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Back to Lobby!");
            SceneManager.LoadScene("LobbyScene_bbbb");
        }
    }
}