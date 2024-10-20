using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

namespace SourGrape.bbbb
{
    public class GameLauncher : MonoBehaviourPunCallbacks
    {
        // game scene(테스트용 씬) 내부에 있음
        #region const
        bool DEBUG = true;
        string PathMap = "bbbb/Maps/";
        string PathPlayable = "bbbb/Playable/";
        #endregion

        #region public variable
        public GameObject Map;
        public PhotonView playable;
        public GameObject playerCam;
        #endregion

        #region private variable
        private int _sendRate = 60;
        private int _readyPlayer = 1;
        private bool _allReady = false;
        private GameObject _cam;
        #endregion

        private void Awake()
        {
            if (DEBUG)
            {
                Debug.Log("bbbb/Maps/" + Map.name);
                PhotonNetwork.ConnectUsingSettings();
            }

        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (_allReady && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("timer start");
                if (_cam != null)
                {
                    _cam.transform.Find("Manager").GetComponent<PhotonView>().RPC("TimerStart", RpcTarget.All);
                    _allReady = false;
                }
            }
        }

        #region public function
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            PhotonNetwork.JoinRandomOrCreateRoom();
            Debug.Log("Connected to Lobby");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("In room!");
            PhotonNetwork.Instantiate(PathMap + Map.name, Vector3.zero, Quaternion.identity);

            SpawnCharacter();
        }
        #endregion

        private void SpawnCharacter()
        {
            Vector3 randPos = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
            GameObject player = PhotonNetwork.Instantiate(PathPlayable + playable.name, randPos, Quaternion.identity);
            _cam = Instantiate(playerCam);
            while(_cam == null)
            {

            }
            photonView.RPC("readyAccept", RpcTarget.All);
        }

        [PunRPC]
        private void readyAccept()
        {
            _readyPlayer--;
            if(_readyPlayer== 0) _allReady = true;
        }
    }
}
