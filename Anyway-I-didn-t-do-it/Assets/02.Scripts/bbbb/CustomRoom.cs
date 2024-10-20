using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using PhotonHTable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using SourGrape.bbbb;
using static SourGrape.bbbb.LobbyLauncher;


namespace SourGrape.bbbb
{
    public class CustomRoom : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI Title;

        public Sprite Crown;
        public Sprite notCrown;

        public GameObject ErrorPanelPrefab;

        [SerializeField] private GameObject CharacterBtnObj;
        [SerializeField] private GameObject BtnStart;

        [SerializeField] private TextMeshProUGUI _name1;
        [SerializeField] private TextMeshProUGUI _name2;
        [SerializeField] private TextMeshProUGUI _name3;
        [SerializeField] private TextMeshProUGUI _name4;

        [SerializeField] private GameObject _spawnPoint1;
        [SerializeField] private GameObject _spawnPoint2;  
        [SerializeField] private GameObject _spawnPoint3;
        [SerializeField] private GameObject _spawnPoint4;

        [SerializeField] private GameObject _mark1;
        [SerializeField] private GameObject _mark2;
        [SerializeField] private GameObject _mark3;
        [SerializeField] private GameObject _mark4;

        [SerializeField] private GameObject _lookPoint;

        private List<GameObject> SpawnPoints = new List<GameObject>();
        private List<GameObject> Marks = new List<GameObject>();
        private List<GameObject> Characters = new List<GameObject>();
        private List<TextMeshProUGUI> Names = new List<TextMeshProUGUI>();

        private PhotonHTable HTable = new PhotonHTable();
        private int CharacterMax = 19;

        private GameObject[] CharacterPrefabs;

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                Title.text = PhotonNetwork.CurrentRoom.Name;
            }
            else
            {
                Debug.Log("No Photon Network");
            }
            SpawnPoints.Add(_spawnPoint1);
            SpawnPoints.Add(_spawnPoint2);
            SpawnPoints.Add(_spawnPoint3);
            SpawnPoints.Add(_spawnPoint4);
            Marks.Add(_mark1);
            Marks.Add(_mark2);
            Marks.Add(_mark3);
            Marks.Add(_mark4);
            Names.Add(_name1);
            Names.Add(_name2);
            Names.Add(_name3);
            Names.Add(_name4);

            HTable["Character"] = 0;
            CharacterPrefabs = Resources.LoadAll<GameObject>("bbbb/Characters");
            
            CustomSetting();
        }

        private void Update()
        {
            BtnStart.SetActive(PhotonNetwork.IsMasterClient);
        }

        private void PlayerSetting()
        {
            Debug.Log("Setting Start");
            Player[] _Players = PhotonNetwork.PlayerList;


            for(int i=0; i< 4; i++)
            {
                Names[i].text = "";
            }

            for (int i = 0; i < Characters.Count; i++)
            {
                Destroy(Characters[i]);
            }
            Characters.Clear();

            for (int i = 0; i < Marks.Count; i++)
            {
                Marks[i].SetActive(false);
            }

            Debug.Log($"player Count : {_Players.Length}");
            for (int i = 0; i < _Players.Length; i++)
            {
                if (_Players[i].NickName == PhotonNetwork.NickName)
                {
                    CharacterBtnObj.transform.position = Marks[i].transform.position;
                    CharacterBtnObj.transform.position += new Vector3(0,-30, 0);
                }
                Marks[i].SetActive(true);
                if (_Players[i].IsMasterClient)
                {
                    Marks[i].GetComponent<Image>().sprite = Crown;
                }
                else
                {
                    Marks[i].GetComponent<Image>().sprite = notCrown;
                }
                
                if (_Players[i].CustomProperties["Character"] != null)
                {
                    Names[i].text = $"{_Players[i].NickName}";
                    GameObject _obj = Instantiate(CharacterPrefabs[(int)(_Players[i].CustomProperties["Character"])], SpawnPoints[i].transform.position , Quaternion.identity);
                    _obj.transform.LookAt(_lookPoint.transform);
                    Characters.Add(_obj);
                }
                else
                {
                    Debug.Log("Not Idx!");
                }
                int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
                string characterKey = "Character" + actNum;
                string characterValue = PlayerPrefs.GetString("CharacterName");
                PhotonHTable character = new PhotonHTable();
                character[characterKey] = characterValue;
                PhotonNetwork.CurrentRoom.SetCustomProperties(character);
            }
        }
        
        public void BtnCharacterPrev()
        {
            int temp = (int)HTable["Character"]; temp--;
            if (temp < 0) temp = CharacterMax;
            HTable["Character"] = temp;
            CustomSetting();
            PlayerPrefs.SetString("CharacterName", CharacterPrefabs[temp].name);
            PlayerPrefs.SetInt("Character", temp);
            PlayerPrefs.Save();
        }

        public void BtnCharacterNext()
        {
            int temp = (int)HTable["Character"]; temp++;
            if (temp < 0) temp = CharacterMax;
            HTable["Character"] = temp;
            CustomSetting();
            PlayerPrefs.SetString("CharacterName", CharacterPrefabs[temp].name);
            PlayerPrefs.SetInt("Character", temp);
            PlayerPrefs.Save();
        }

        public void BtnToLobby()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void BtnStartFunc()
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount != Parameters.MaxPlayer)
            {
                GameObject _obj = Instantiate(ErrorPanelPrefab);
                _obj.GetComponent<ErrorPanel>().message = Parameters.NeedMorePlayers;
                return;
            }
            PhotonHTable roomHash = new PhotonHTable();
            LobbyLauncher _temp = new LobbyLauncher();
            roomHash["Stage"] = 0;
            roomHash["Maps"] = _temp.GetRandomThree();
            roomHash["Custom"] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);// �� ��������, ������ �� 3�� ����
            PhotonNetwork.CurrentRoom.IsOpen = false;// �߰��� �ű� ���� ������ �Ұ��ϰ�

            int[] randomMapIndices = (int[])roomHash["Maps"];
            EMap nextMap = (EMap)randomMapIndices[0]; // ù ��° �� ����

            PhotonNetwork.LoadLevel(nextMap.ToString()); // �� �ε�
                                                         //PhotonNetwork.LoadLevel("DesertMap");
        }

        public void BtnCopyCode()
        {
            GUIUtility.systemCopyBuffer = Title.text;
            GameObject _obj = Instantiate(Resources.Load<GameObject>(Parameters.ErrorPanel));
            _obj.GetComponent<bbbb.ErrorPanel>().message = Parameters.MsgCopyCode;
        }


        private void CustomSetting()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(HTable);
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            HTable["Character"] = 0;
            CustomSetting();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            HTable["Character"] = 0;
            CustomSetting();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log("Setting Trigger 1");

            PlayerSetting();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log("Setting Trigger 2");
            PlayerSetting();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHTable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            Debug.Log("Setting Trigger 3");
            PlayerSetting();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene("LobbyScene_bbbb");
        }

    }
}
