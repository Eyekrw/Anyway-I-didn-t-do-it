//using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using PhotonHTable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SourGrape.bbbb
{
    public class LobbyLauncher : MonoBehaviourPunCallbacks
    {
        #region public Variable
        public Canvas loadingCanvas;
        public GameObject MatchingPanel;  //Y: 80 <-> -75
        public GameObject MatchingBtnPanel;
        public GameObject CancelPanel;
        public TMPro.TMP_InputField CustomCode;
        public TMPro.TextMeshProUGUI userInfoText;
        public TMPro.TextMeshProUGUI pingText;
        public TMPro.TextMeshProUGUI matchingText;
        public enum EMap { IceMap, DesertMap, FireMap, RiverMap, MirrorMap, WindMap, SwordMap }
        #endregion
        #region private Variable
        private GameObject PlayerData;
        private float _TargetTime;
        private bool _OnMatch = false;
        private int _MaxPlayer = Parameters.MaxPlayer;
        private int _sendRate = 20;
        private int _SerializationRate = 20;
        private bool _TryCreateCustom = false;
        private bool _CustomMode = false;

        private PhotonHTable hashtable = new PhotonHTable();

        private bool DEBUG = false;
        private List<char> CHARS = new List<char>();

        #endregion

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.SendRate = _sendRate;
            PhotonNetwork.SerializationRate = _SerializationRate;
            //PhotonNetwork.ConnectToRegion("kr");
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
        }
        void Start()
        {
            MatchingPanel.SetActive(false);
            loadingCanvas.planeDistance = 10;
            _TargetTime = Time.time + 1.0f;

            PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            string url = Parameters.URL + "api/rank/mine/" + PhotonNetwork.NickName;
            StartCoroutine(GetMyWinCnt(url));
        }

        void Update()
        {
            if (Time.time > _TargetTime)
            {
                pingText.text = $"ping : {PhotonNetwork.GetPing()} ms  Server: {PhotonNetwork.CloudRegion}";
                _TargetTime = Time.time + 1.0f;

                //Debug.Log($"ttl : {PhotonNetwork.CurrentRoom.PlayerTtl}");
                //Debug.Log($"playerCount : {PhotonNetwork.CurrentRoom.PlayerCount}");
            }
            if (PhotonNetwork.InRoom)
            {
                matchingText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {_MaxPlayer}";
            }
        }

        #region override Function
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log("Connected to Lobby");
            loadingCanvas.planeDistance = -1;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("In room!");

            if (_CustomMode)
            {
                SceneManager.LoadScene("CustomRoom");
            }
            else
            {
                MatchingPanel.SetActive(true);
                if(PhotonNetwork.PlayerList.Length == 4)
                {
                    SetCharacterAvartar();
                }
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            }
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            if (_TryCreateCustom)
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                SceneManager.LoadScene("CustomRoom");
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            if (returnCode == ErrorCode.GameIdAlreadyExists)
            {
                // 방 이름이 이미 존재하는 경우, 새로운 방 이름 생성 후 다시 시도
                string newRoomName = GetRoomCode();  // 새로운 방 이름 생성
                BtnCustomCreate();
            }
            else
            {
                _TryCreateCustom = false;
            }
        }

        private void SetCharacterAvartar()
        {
            // save player prefab name to PhotonNetwork.CurrentRoom
            int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
            string characterKey = "Character" + actNum;
            // Debug.Log($"CharacterKey : {characterKey}");
            string characterValue = PlayerPrefs.GetString("CharacterName");
            // Debug.Log($"CharacterName : {characterValue}");
            PhotonHTable character = new PhotonHTable();
            character[characterKey] = characterValue;
            PhotonNetwork.CurrentRoom.SetCustomProperties(character);
        }
        
        private IEnumerator GameStart()
        {
            SetCharacterAvartar();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonHTable roomHash = new PhotonHTable();
                roomHash["Stage"] = 0;
                roomHash["Maps"] = GetRandomThree();
                roomHash["Custom"] = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);// �� ��������, ������ �� 3�� ����
                PhotonNetwork.CurrentRoom.IsOpen = false;// �߰��� �ű� ���� ������ �Ұ��ϰ�

                int[] randomMapIndices = (int[])roomHash["Maps"];
                EMap nextMap = (EMap)randomMapIndices[0]; // ù ��° �� ����

                yield return new WaitForSeconds(2f);
                PhotonNetwork.LoadLevel(nextMap.ToString()); // �� �ε�
                                                             //PhotonNetwork.LoadLevel("DesertMap");
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer) //������ �濡 ����
        {
            base.OnPlayerEnteredRoom(newPlayer);
            if (PhotonNetwork.CurrentRoom.PlayerCount == _MaxPlayer )
            {
                StartCoroutine(GameStart());
            }
        }
        

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Back to Lobby!");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log("someone left");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            if (_CustomMode)
            {
                GameObject _obj = Instantiate(Resources.Load<GameObject>(Parameters.ErrorPanel));
                _obj.GetComponent<bbbb.ErrorPanel>().message = Parameters.WrongRoomCode;
                _CustomMode = false;
            }
        }

        #endregion

        #region Button Function
        public void BtnRandomMatch()
        {
            if (PhotonNetwork.InRoom) return;
            MatchingBtnPanel.SetActive(false);
            CancelPanel.SetActive(true);
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = _MaxPlayer;
            ro.IsVisible = true;
            ro.IsOpen = true;
            ro.CleanupCacheOnLeave =true;
            ro.EmptyRoomTtl = 1000;
            ro.PlayerTtl = 10;

            PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, null, ro);
        }

        public void BtnMatchCancel()
        {
            _OnMatch = false;
            MatchingPanel.SetActive(false);
            MatchingBtnPanel.SetActive(true);
            CancelPanel.SetActive(false);
            if (!PhotonNetwork.InRoom) return;
            PhotonNetwork.LeaveRoom();
        }

        public void BtnCustomCreate()
        {
            Debug.Log("Custom Create Btn!");
            if (PhotonNetwork.InRoom) return;

            string _code = GetRoomCode();
            _TryCreateCustom = true;
            try
            {
                MatchingBtnPanel.SetActive(false);
                CancelPanel.SetActive(true);
                RoomOptions ro = new RoomOptions();
                ro.MaxPlayers = _MaxPlayer;
                ro.IsOpen = true;   // enter available
                ro.IsVisible = false;    // random enter disavailable
                ro.CleanupCacheOnLeave = true;
                ro.EmptyRoomTtl = 1000;
                ro.PlayerTtl = 10;

                PhotonNetwork.CreateRoom(_code, ro, null, null);
            }
            catch
            {
                Debug.Log("Failed Create Room");
            }
        }

        public void BtnCustomEnter()
        {
            Debug.Log("Custom Enter Btn");
            if(CustomCode.text.Length != 8)
            {
                Debug.Log("Wrong Code!");
            }

        }

        public void BtnEnterMatch()
        {
            if(CustomCode.text.Length != 8)
            {
                Debug.Log("Code is not available");
                GameObject _obj = Instantiate(Resources.Load<GameObject>(Parameters.ErrorPanel));
                _obj.GetComponent<bbbb.ErrorPanel>().message = Parameters.WrongRoomCode;
                return;
            }
            Debug.Log($"Room Code : {CustomCode.text}");
            try
            {
                _CustomMode = true;
                PhotonNetwork.JoinRoom(CustomCode.text);
            }
            catch
            {
                GameObject _obj =Instantiate(Resources.Load<GameObject>(Parameters.ErrorPanel));
                _obj.GetComponent<bbbb.ErrorPanel>().message = Parameters.WrongRoomCode;
                _CustomMode = false;
            }
        }

        public int[] GetRandomThree()// 0~6 select random three map.
        {
            HashSet<int> uniqueNumbers = new HashSet<int>();
            System.Random random = new System.Random();

            while (uniqueNumbers.Count < 3)
            {
                int number = random.Next(0, 7); // 0���� 3������ ���� ���� ����
                uniqueNumbers.Add(number);
            }

            return new List<int>(uniqueNumbers).ToArray();
            //return new int[] { (int)EMap.WindMap, (int)EMap.FireMap, (int)EMap.MirrorMap };
        }

        #endregion

        #region private Function
        private IEnumerator GetMyWinCnt(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("JWT")}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string ret = request.downloadHandler.text;
                try 
                {
                    PlayerRankInfo rankInfo = JsonConvert.DeserializeObject<PlayerRankInfo>(ret);
                    userInfoText.text = "닉네임: " + PhotonNetwork.NickName + "\n이긴 횟수: " + rankInfo.winCnt;
                }
                catch
                {
                    userInfoText.text = "닉네임: " + PhotonNetwork.NickName + "\n이긴 횟수: 0";
                    Debug.Log("error");
                }
            }
            else
            {
                Debug.LogError("fail" + request.error);
            }
        }


        private string GetRoomCode()
        {
            if (CHARS.Count == 0)
                fillChars();

            StringBuilder _ret = new StringBuilder(8);  // 8자리 코드 생성

            for (int i = 0; i < 8; i++)
            {
                int idx = Random.Range(0, CHARS.Count);
                _ret.Append(CHARS[idx]);
            }

            return _ret.ToString();
        }

        private void fillChars()
        {
            for (int i = 0; i < 26; i++)
            {
                CHARS.Add((char)('a' + i));
            }
            for(int i=0; i<10; i++)
            {
                CHARS.Add((char)('a' + i));
            }
        }

        #endregion
    }
}