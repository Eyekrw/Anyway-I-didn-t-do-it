using UnityEngine;
using System;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using SourGrape.bbbb;

using PhotonHTable = ExitGames.Client.Photon.Hashtable;
using SourGrape.hongyeop;
using static SourGrape.bbbb.LobbyLauncher;
using System.Globalization;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;

// ���� �г���, ������ ĳ���͸� PlayerPrefs�� ���� <-- �̰� ����. ���� �����Ǹ� ���� �ڱ� �г����̶� ������ ĳ���� ���̵� RPC�� ��ο��� ������.
// Ư�� �������� ���� �͵� RPC�� ����. �׸��� �̰Ŵ� ������Ʈ ������ �г����̶� ���ϸ鼭...��Ե� RPC�� �սô�.

namespace SourGrape.kiyoung
{
    public class GameManager : MonoBehaviourPun
    {
        #region public fields
        public float RemainingTime = 0;    // Ÿ�̸� ���� �ð�
        public float ElapsedTime = -1;      // Ÿ�̸� ����ð�
        public bool IsModalUp = false;
        public int DeathCount = 0;  // ���ӿ��� ���� ��� ��
        //public int[] DeathOrder = { 4, 4, 4, 4 };    // ���� ����. 5�� �ֵ��� ����������. 1, 2, 3 ������ ��������.
        public int[] DeathOrder = { -1, -1, -1, -1 };
        public Dictionary<int, bool> IsDead = new Dictionary<int, bool>();
        //public bool[] IsDead = {false, false, false, false};
        public bool _isRoundOver = false;
        public List<GameObject> Winners; // winner players gameobjects for camera end controller
        public AudioClip newBGMClip;
        #endregion
        #region Serialized fields
        [SerializeField]
        private float _roundTime = 5f; // �� ���� �� �ð�
        [SerializeField]
        private int _playerCount = Parameters.MaxPlayer;   // �÷��̾� ��
        #endregion

        #region private fields
        private GameObject _mainCamera;
        private CameraController _cameraController;
        private GameObject _uiCamera;
        private GameObject _roundOver;
        private TextMeshProUGUI _timerText;
        private bool _isTimerRunning = false;    // ���� �������ΰ�
        private double _timerStartTime = -1;
        private GameObject[] _prefabs;
        private PhotonHTable hashtable = new PhotonHTable();
        private GameObject _bgSound;
        private AudioSource _bgAudioSource;
        private string[] animations;
        #endregion

        void Start()
        {
            
            Initialize();   // ���� �ʱ� ����
            _bgSound = GameObject.Find("BGSound");
            _bgAudioSource = _bgSound.GetComponent<AudioSource>();
            SpawnPlayer();  // Ŭ���̾�Ʈ ���� ĳ���� ����
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.InstantiateRoomObject("bbbb/ArrowManager", Vector3.zero, Quaternion.identity);
                //arrowSpawner();   
                transform.GetComponent<ArrowManager>().enabled = true;
            }
            TimerSetting(); // Ÿ�̸Ӽ���
        }
        void Update()
        {
            if (_isTimerRunning)
            {
                ElapsedTime = (float)(PhotonNetwork.Time - _timerStartTime);
                RemainingTime = _roundTime - ElapsedTime;
                if (RemainingTime >= 0)
                    UpdateTimerUI();
                else
                    _isTimerRunning = false;
            }
            // Ÿ�̸Ӱ� �����ų� 4�� �� 3���� ������ ���� ����
            if (!_isRoundOver && (RemainingTime < 0 || DeathCount >= _playerCount - 1))
            {
                _isRoundOver = true;
                RoundOver();
            }
        }


        private void Initialize()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            RemainingTime = -1;    // Ÿ�̸� ���� �ð�
            ElapsedTime = -1;      // Ÿ�̸� ����ð�
            IsModalUp = false;
            DeathCount = 0;  // ���ӿ��� ���� ��� ��
            //DeathOrder = new int[4];
            //DeathOrder = new int[4];
            //for (int i = 0; i < 4; i++) {
            //    DeathOrder[i] = 4;
            //}

            _roundTime = Parameters.GameTime; // �� ���� �� �ð�
            _playerCount = Parameters.MaxPlayer;   // �÷��̾� ��

            _mainCamera = GameObject.Find("Main Camera");
            _cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
            _uiCamera = GameObject.Find("UI Camera").gameObject;
            _roundOver = GameObject.Find("RoundOver").gameObject;
            _roundOver.SetActive(false);
            _timerText = GameObject.Find("Timer").gameObject.GetComponent<TextMeshProUGUI>();
            _prefabs = Resources.LoadAll<GameObject>("bbbb/Characters");

            animations = new string[5];
            animations[0] = "Jump";
            animations[1] = "Hello";
            animations[2] = "Hiding";
            animations[3] = "Dance";
            animations[4] = "Attack";
        }
        private void SpawnPlayer()
        {
            // ��������Ʈ �ӽ� ����(�߾� ��ó������)
            //Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
            Vector3[] spawnPositions = new Vector3[_playerCount]; // if it doesn't work, delete this line
            spawnPositions[0] = GameObject.Find("Respawn1").transform.position;// if it doesn't work, delete this line
            spawnPositions[1] = GameObject.Find("Respawn2").transform.position;// if it doesn't work, delete this line
            spawnPositions[2] = GameObject.Find("Respawn3").transform.position;// if it doesn't work, delete this line
            spawnPositions[3] = GameObject.Find("Respawn4").transform.position;// if it doesn't work, delete this line
            string characterName = PlayerPrefs.GetString("CharacterName"); // load sort of character from PlayuerPrefs
            //GameObject player = PhotonNetwork.Instantiate("Player/Charaters/" + characterName, spawnPosition, Quaternion.identity);
            // �����÷��̾� �±׿�����Ʈ�� ������ �÷��̾ �Ҵ�
            //PhotonNetwork.LocalPlayer.TagObject = player;  // ��� ���°���?

            // ���� �÷��̾��� ĳ���;��̵�, �г����� �� Ŀ����������Ƽ�� ����
            // ���� �÷��̾�� PlayerIdx�� ����
            hashtable["Character"] = PlayerPrefs.GetInt("Character");
            hashtable["NickName"] = PlayerPrefs.GetString("NickName");
            int playerIndex = -1; ;// if it doesn't work, delete this line
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
                {
                    playerIndex = i;
                    hashtable["PlayerIdx"] = i;
                    break;
                }
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
            Vector3 spawnPosition = spawnPositions[playerIndex];
            GameObject player = PhotonNetwork.Instantiate("Player/Charaters/" + characterName, spawnPosition, Quaternion.identity);
            _cameraController.enabled = true;
            // find and assign map border to player character
            GameObject _mapBorder = GameObject.Find("MapBorder");
            // PhotonView를 통해 로컬 플레이어인지 확인
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                // MapBorder에 로컬 플레이어의 Transform 설정
                Debug.Log("MapBorder에 로컬 플레이어의 Transform 할당: " + player.transform.name);
                _mapBorder.GetComponent<MapBorderMaterialSwitcher>().SetPlayer(player.transform);
            }
        }
        #region Timer-related Functions
        private void TimerSetting()
        {
            _isTimerRunning = true;
            _timerStartTime = PhotonNetwork.Time;
            RemainingTime = _roundTime;
        }

        private void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(RemainingTime / 60);
            int seconds = Mathf.FloorToInt(RemainingTime % 60);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        #endregion
        #region RoundOver-related Functions
        private void RoundOver()
        {
            if (!IsModalUp)  // ī�޶� 1�δ� 5�ʾ� �����ڵ� ��� ���� ����
            {
                IsModalUp = true;
                StartCoroutine(ModalSetting());
            }
        }
        private IEnumerator ModalSetting()
        {
            yield return new WaitForSeconds(5f);
            ClientRoundOver(); // client 
            yield return new WaitForSeconds(7f);
            _bgAudioSource.Stop();
            _bgAudioSource.clip = newBGMClip;
            _bgAudioSource.Play();
            _roundOver.SetActive(true);  // ���â�� ���

            int d = DeathCount;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (!IsDead.ContainsKey(p.ActorNumber))
                {
                    DeathOrder[d] = p.ActorNumber;
                    d++;
                    //DeathOrder  p.ActorNumber.ToString();
                }
            }

            int[] modalOrder = new int[_playerCount];
            int j = 0;
            for(int i = DeathOrder.Length - 1; i >= 0; i--)
            {
                modalOrder[j++] = DeathOrder[i];
            }
            Transform roundResult = _roundOver.transform.GetChild(0);
            for (int i = 0; i < modalOrder.Length; i++)
            {
                int now = modalOrder[i];
                foreach(Player p in PhotonNetwork.PlayerList)
                {
                    int pActNum = p.ActorNumber;
                    if(now == pActNum)
                    {
                        GameObject player = roundResult.GetChild(i).gameObject;
                        TextMeshProUGUI nickname = player.transform.Find("NamePanel").GetComponentInChildren<TextMeshProUGUI>();
                        TextMeshProUGUI grade = player.transform.Find("Grade").GetComponentInChildren<TextMeshProUGUI>();
                        if (p.CustomProperties.TryGetValue("NickName", out object name))
                        {
                            nickname.text = (string)name;
                        }
                        if (p.CustomProperties.TryGetValue("Character", out object cId))
                        {
                            Vector3 playerPos = player.transform.position;
                            playerPos.z -= 1;
                            playerPos.y -= 1.3f;
                            GameObject playerModel = Instantiate(_prefabs[(int)cId], playerPos, Quaternion.Euler(0, 180, 0));
                            int randNum = UnityEngine.Random.Range(0, 5);
                            string prefabName = _prefabs[(int)cId].name;
                            string characterType = "";
                            for (int k = 0; k < prefabName.Length; k++)
                            {
                                if (prefabName[k] == ' ' || prefabName[k] == '\0')
                                    break;
                                characterType += prefabName[k];
                            }
                            string animationName = characterType + "_" + animations[randNum];
                            playerModel.transform.GetComponent<Animator>().Play(animationName);
                            playerModel.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                            SetLayerRecursively(playerModel, "UI");
                        }

                        grade.text = (i+1).ToString();
                        break;
                    }
                }
            }
            int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
            string scoreKey = "Score" + actNum;
            int newScore = 0;
            if (IsDead.ContainsKey(actNum) && IsDead[actNum])
            {
                int deathOrder = 0;
                for(int i = 0; i < _playerCount; i++)
                {
                    if(actNum == modalOrder[i])
                    {
                        deathOrder = _playerCount - i;
                        break;
                    }
                }
                newScore = deathOrder * 100;
            }
            else
            {
                newScore = 500;
            }
            int oldScore = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(scoreKey))
            {
                oldScore = (int)PhotonNetwork.CurrentRoom.CustomProperties[scoreKey];
            }

            PhotonHTable score = new PhotonHTable();
            score[scoreKey] = newScore + oldScore;
            Debug.Log("Old: " + oldScore + "/ New: " + newScore);
            PhotonNetwork.CurrentRoom.SetCustomProperties(score);

            yield return new WaitForSeconds(5);
            // ���⼭ ���� ��������� �Ѿ����. _roundOver�� �� �ʿ䵵 ����.
            //_roundOver.SetActive(false);
            //Debug.Log("????");
            if (PhotonNetwork.IsMasterClient)
            {
                //Debug.Log("Im master");
                bool isAllRoundOver = false;  // ���⼭ ���� ���� �� ������Ƽ�� ������ ���̶� ������ ����
                PhotonHTable currentStage = new PhotonHTable();
                int currentStageNum = (int)PhotonNetwork.CurrentRoom.CustomProperties["Stage"];
                // ��� ���尡 �������� Ȯ��
                if (currentStageNum >= 2)
                {
                    isAllRoundOver = true;
                }
                else
                {
                    // ���� ����� �Ѿ�� ���� Stage ����
                    currentStageNum++;
                    currentStage["Stage"] = currentStageNum;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(currentStage);
                }

                // ���ǿ� ���� ���� ������ �̵�
                if (isAllRoundOver)
                {
                    PhotonNetwork.LoadLevel("GameResult");
                }
                else
                {
                    EMap nextMap = (EMap)((int[])PhotonNetwork.CurrentRoom.CustomProperties["Maps"])[currentStageNum];
                    PhotonNetwork.LoadLevel(nextMap.ToString());
                }
            }
        }
        private void SetLayerRecursively(GameObject obj, string newLayer)
        {
            // ���� ������Ʈ�� ���̾� ����
            obj.layer = LayerMask.NameToLayer(newLayer);

            // ��� �ڽ� ������Ʈ�� ���� ��� ȣ��
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }


        private void ClientRoundOver()
        {
            FindWinners(); // find winner players gameObject for camera end controller
            WinnersDance(); // set winner players gameObject.IsDance
            GhostsBuster(); // set ghost players gameObject.IsDead
            _mainCamera.GetComponent<CameraController>().enabled = false; // unable player camera controller
            _mainCamera.GetComponent<CameraGhostController>().enabled = false; // unable ghost camera controller
            _mainCamera.GetComponent<GameEndCameraManager>().enabled = true; // enable game end camera controller
        }

        private void FindWinners()
        {
            // �±װ� "Player"�� ��� ������Ʈ�� ã�Ƽ� �迭�� ����
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            Winners = new List<GameObject>();

            foreach (GameObject player in allPlayers)
            {
                PlayerStatusController status = player.GetComponent<PlayerStatusController>();
                if (status != null && !status.IsDead)
                {
                    Winners.Add(player);
                }
            }
            // Debug.Log("Number of winners: " + Winners.Count);
        }

        private void WinnersDance()
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in allPlayers)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    PlayerStatusController status = player.GetComponent<PlayerStatusController>();
                    if (status != null)
                    {
                        status.IsDance = true;
                    }
                }
            }
        }

        private void GhostsBuster()
        {
            GameObject[] allGhosts = GameObject.FindGameObjectsWithTag("PlayerGhost");
            foreach (GameObject ghost in allGhosts)
            {
                if (ghost.GetComponent<PhotonView>().IsMine)
                {
                    PlayerGhostStatusController status = ghost.GetComponent<PlayerGhostStatusController>();
                    if (status != null)
                    {
                        status.IsDead = true;
                    }
                }
            }
        }

        #endregion
        #region Deprecated Functions
        //private void EndGame()
        //{
        //    _timerText.text = string.Format("Game Over! Winner!");
        //    //_isGameRunning = false;
        //    StopAllCoroutines();
        //}
        #endregion
    }
}